using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;

namespace order.Services
{
    public class OrderConsumer : BackgroundService
    {
        private ServiceBusProcessor _processor;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = File.ReadAllText("/mnt/secrets/servicebus-connection").Trim();
            var queue = "order-queue";

            var client = new ServiceBusClient(connection);

            _processor = client.CreateProcessor(queue);

            _processor.ProcessMessageAsync += HandleMessage;
            _processor.ProcessErrorAsync += HandleError;

            Console.WriteLine("🚀 Order Service started");

            await _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task HandleMessage(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();

            Console.WriteLine($"📦 Order received raw: {body}");

            try
            {
                var order = JsonSerializer.Deserialize<object>(body);

                // 💥 FORCE ERROR
                throw new Exception("💥 Forced failure for DLQ");

                // never exceuted
                // await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing order: {ex.Message}");

                // 🔥 IMPORTANT → laisse l’exception remonter
                throw;
            }
        }

        private Task HandleError(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"❌ ServiceBus Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}