using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace catalog.Controllers
{
    [ApiController]
    [Route("catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CatalogController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("test-cart")]
        public async Task<IActionResult> TestCart()
        {
            var client = new HttpClient();

            var response =
                await client.GetStringAsync(
                    "http://cart-service/cart");

            return Ok(response);
        }

        [HttpGet("send-message")]
        public async Task<IActionResult> SendMessage()
        {
            string connectionString;

            // AKS mounted secret via CSI Driver + Azure Key Vault
            if (System.IO.File.Exists("/mnt/secrets/servicebus-connection"))
            {
                connectionString = System.IO.File
                    .ReadAllText("/mnt/secrets/servicebus-connection")
                    .Trim();

                Console.WriteLine(
                    "🔐 Using Service Bus connection from mounted secret");
            }
            else
            {
                // Local development fallback
                connectionString =
                    Environment.GetEnvironmentVariable(
                        "SERVICEBUS_CONNECTION")
                    ?? _configuration["ServiceBus:ConnectionString"]
                    ?? throw new Exception(
                        "Service Bus connection string not found");

                Console.WriteLine(
                    "💻 Using Service Bus connection from environment variable");
            }

            string queueName;

            // AKS mounted secret for queue name
            if (System.IO.File.Exists("/mnt/secrets/servicebus-queue"))
            {
                queueName = System.IO.File
                    .ReadAllText("/mnt/secrets/servicebus-queue")
                    .Trim();

                Console.WriteLine(
                    "🔐 Using queue name from mounted secret");
            }
            else
            {
                // Local development fallback
                queueName =
                    Environment.GetEnvironmentVariable(
                        "SERVICEBUS_QUEUE")
                    ?? _configuration["ServiceBus:QueueName"]
                    ?? throw new Exception(
                        "Service Bus queue name not found");

                Console.WriteLine(
                    "💻 Using queue name from environment variable");
            }

            await using var client =
                new ServiceBusClient(connectionString);

            var sender =
                client.CreateSender(queueName);

            // Correlation ID for distributed tracing
            var correlationId =
                Guid.NewGuid().ToString();

            var payload = new
            {
                Product = "New order from catalog",
                CorrelationId = correlationId
            };

            var json =
                JsonSerializer.Serialize(payload);

            var message =
                new ServiceBusMessage(json);

            Console.WriteLine(
                $"📦 [SENDING] CorrelationId={correlationId}");

            await sender.SendMessageAsync(message);

            Console.WriteLine(
                $"✅ [SENT] CorrelationId={correlationId}");

            return Ok(new
            {
                Status = "Message sent",
                CorrelationId = correlationId
            });
        }
    }
}