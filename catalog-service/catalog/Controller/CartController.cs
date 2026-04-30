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
            var response = await client.GetStringAsync("http://cart-service/cart");

            return Ok(response);
        }


        [HttpGet("send-message")]
        public async Task<IActionResult> SendMessage()
        {
            var connectionString =
                Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION")
                ?? _configuration["ServiceBus:ConnectionString"];

            var queueName =
                Environment.GetEnvironmentVariable("SERVICEBUS_QUEUE")
                ?? _configuration["ServiceBus:QueueName"];

            await using var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            // 🔗 Correlation ID
            var correlationId = Guid.NewGuid().ToString();

            var payload = new
            {
                Product = "New order from catalog",
                CorrelationId = correlationId
            };

            var json = JsonSerializer.Serialize(payload);

            var message = new ServiceBusMessage(json);

            await sender.SendMessageAsync(message);

            Console.WriteLine($"🟢 Catalog sent | CorrelationId: {correlationId}");

            return Ok(new
            {
                Status = "Message sent",
                CorrelationId = correlationId
            });
        }
    }
}