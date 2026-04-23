using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

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
            // var connectionString = _configuration["ServiceBus:ConnectionString"];
            // var queueName = _configuration["ServiceBus:QueueName"];

            var connectionString =
                Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION")
                ?? _configuration["ServiceBus:ConnectionString"];

            var queueName =
                Environment.GetEnvironmentVariable("SERVICEBUS_QUEUE")
                ?? _configuration["ServiceBus:QueueName"];

            await using var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var message = new ServiceBusMessage("New order from catalog");

            await sender.SendMessageAsync(message);

            return Ok("Message sent to Service Bus");
        }
    }
}