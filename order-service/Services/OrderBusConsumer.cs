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
        private ServiceBusProcessor? _processor;
        private ServiceBusClient? _client;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var connection = File.ReadAllText("/mnt/secrets/servicebus-connection").Trim();
                var queue = "order-queue";

                _client = new ServiceBusClient(connection);

                _processor = _client.CreateProcessor(queue, new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 2
                });

                _processor.ProcessMessageAsync += HandleMessage;
                _processor.ProcessErrorAsync += HandleError;

                Console.WriteLine("🚀 Order Service started");
                Console.WriteLine("📡 Connected to ServiceBus");
                Console.WriteLine($"📥 Listening on queue: {queue}");

                Console.WriteLine("🔥 Initializing ServiceBus Processor...");
                await _processor.StartProcessingAsync(stoppingToken);

                Console.WriteLine("👂 Order listening to queue...");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Startup error: {ex.Message}");
                throw;
            }
        }

        private async Task HandleMessage(ProcessMessageEventArgs args)
        {
            var raw = args.Message.Body.ToString();
            Console.WriteLine($"📦 [RECEIVED] {raw}");

            string? correlationId = null;

            try
            {
                var incoming = JsonSerializer.Deserialize<JsonElement>(raw);

                var product = incoming.GetProperty("Product").GetString();
                correlationId = incoming.GetProperty("CorrelationId").GetString();

                Console.WriteLine($"🔗 CorrelationId: {correlationId}");
                Console.WriteLine($"📦 Product: {product}");

                Console.WriteLine($"🟢 [PROCESSING] CorrelationId={correlationId}");

                if (!string.IsNullOrEmpty(product) && product.Contains("FAIL"))
                {
                    throw new Exception("Simulated failure");
                }

                Console.WriteLine($"✅ [SUCCESS] CorrelationId={correlationId}");

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ERROR] CorrelationId={correlationId ?? "unknown"} | {ex.Message}");
                Console.WriteLine($"📦 Raw message: {raw}");

                throw;
            }
        }

        private Task HandleError(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"❌ [SB ERROR] {args.Exception.Message}");
            Console.WriteLine($"📍 Entity: {args.EntityPath}");
            Console.WriteLine($"📍 Source: {args.ErrorSource}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("🛑 Stopping Order Service...");

            if (_processor != null)
                await _processor.DisposeAsync();

            if (_client != null)
                await _client.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}