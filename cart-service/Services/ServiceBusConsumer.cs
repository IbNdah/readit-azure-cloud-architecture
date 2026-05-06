using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;

namespace cart.Services
{
    public class ServiceBusConsumer : BackgroundService
    {
        private ServiceBusProcessor _processor;
        private ServiceBusClient _client;
        private ServiceBusSender _sender;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var connectionString = File.ReadAllText("/mnt/secrets/servicebus-connection").Trim();
                var cartQueue = File.ReadAllText("/mnt/secrets/servicebus-queue").Trim();
                var orderQueue = "order-queue";

                Console.WriteLine($"📡 Cart listening to: {cartQueue}");
                Console.WriteLine($"📡 Cart sending to: {orderQueue}");

                _client = new ServiceBusClient(connectionString);

                _processor = _client.CreateProcessor(cartQueue, new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 2
                });

                _sender = _client.CreateSender(orderQueue);

                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;

                Console.WriteLine("🚀 Cart Service started");

                await _processor.StartProcessingAsync(stoppingToken);

                // 🔥 garder le service vivant
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // arrêt normal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Startup error: {ex.Message}");
                throw;
            }
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var raw = args.Message.Body.ToString();
            Console.WriteLine($"🔥 Cart received raw: {raw}");

            try
            {
                var incoming = JsonSerializer.Deserialize<JsonElement>(raw);

                if (!incoming.TryGetProperty("Product", out var productProp) ||
                    !incoming.TryGetProperty("CorrelationId", out var corrProp))
                {
                    throw new Exception("Invalid message schema");
                }

                var product = productProp.GetString();
                var correlationId = corrProp.GetString();

                var order = new
                {
                    OrderId = Guid.NewGuid().ToString(),
                    Product = product,
                    CreatedAt = DateTime.UtcNow,
                    Source = "cart-service",
                    CorrelationId = correlationId
                };

                var json = JsonSerializer.Serialize(order);

                await _sender.SendMessageAsync(new ServiceBusMessage(json));

                Console.WriteLine($"📦 Cart → Order | CorrelationId: {correlationId}");

                // ✅ ACK uniquement après succès complet
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Cart processing error: {ex.Message}");

                // ❌ ne pas ACK → retry + DLQ
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"❌ ServiceBus Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("🛑 Stopping Cart Service...");

            if (_processor != null)
                await _processor.DisposeAsync();

            if (_sender != null)
                await _sender.DisposeAsync();

            if (_client != null)
                await _client.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}