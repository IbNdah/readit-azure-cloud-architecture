using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace cart.Services
{
    public class ServiceBusConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _processor;

        public ServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString =
                Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION")
                ?? _configuration["ServiceBus:ConnectionString"];

            var queueName =
                Environment.GetEnvironmentVariable("SERVICEBUS_QUEUE")
                ?? _configuration["ServiceBus:QueueName"];

            var client = new ServiceBusClient(connectionString);

            _processor = client.CreateProcessor(queueName);

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();

            Console.WriteLine($"🔥 Received message: {body}");

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"❌ Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}