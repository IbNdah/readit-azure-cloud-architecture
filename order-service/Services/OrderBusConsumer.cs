using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace order.Services
{
    public class OrderConsumer : BackgroundService
    {
        private readonly ILogger<OrderConsumer> _logger;

        private ServiceBusProcessor? _processor;
        private ServiceBusClient? _client;

        private static readonly ActivitySource ActivitySource =
            new("readit.order");

        public OrderConsumer(ILogger<OrderConsumer> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            try
            {
                string? connection;

                // AKS mounted secret
                if (File.Exists("/mnt/secrets/servicebus-connection"))
                {
                    connection = File
                        .ReadAllText("/mnt/secrets/servicebus-connection")
                        .Trim();

                    _logger.LogInformation(
                        "🔐 Using Service Bus connection from mounted secret");
                }
                else
                {
                    // Local development fallback
                    connection =
                        Environment.GetEnvironmentVariable(
                            "SERVICEBUS_CONNECTION");

                    _logger.LogInformation(
                        "💻 Using Service Bus connection from environment variable");
                }

                if (string.IsNullOrWhiteSpace(connection))
                {
                    throw new Exception(
                        "Service Bus connection string not found");
                }

                var queue = "order-queue";

                _client = new ServiceBusClient(connection);

                _processor = _client.CreateProcessor(
                    queue,
                    new ServiceBusProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        MaxConcurrentCalls = 2
                    });

                _processor.ProcessMessageAsync += HandleMessage;
                _processor.ProcessErrorAsync += HandleError;

                _logger.LogInformation("🚀 Order Service started");

                _logger.LogInformation(
                    "📡 Connected to Service Bus");

                _logger.LogInformation(
                    "📥 Listening on queue: {Queue}",
                    queue);

                await _processor.StartProcessingAsync(stoppingToken);

                _logger.LogInformation(
                    "👂 Order consumer running");

                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation(
                    "🛑 Order Service stopping...");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Fatal startup error in Order Service");

                throw;
            }
        }

        private async Task HandleMessage(
            ProcessMessageEventArgs args)
        {
            var raw = args.Message.Body.ToString();

            string correlationId = "unknown";
            string product = "unknown";

            using var activity =
                ActivitySource.StartActivity("process-order");

            try
            {
                _logger.LogInformation(
                    "📦 [RECEIVED] MessageId={MessageId}",
                    args.Message.MessageId);

                var incoming =
                    JsonSerializer.Deserialize<JsonElement>(raw);

                // Validate Product
                if (!incoming.TryGetProperty(
                        "Product",
                        out var productElement))
                {
                    await DeadLetterInvalidMessage(
                        args,
                        "MissingProduct",
                        "Product field missing");

                    return;
                }

                // Validate CorrelationId
                if (!incoming.TryGetProperty(
                        "CorrelationId",
                        out var correlationElement))
                {
                    await DeadLetterInvalidMessage(
                        args,
                        "MissingCorrelationId",
                        "CorrelationId field missing");

                    return;
                }

                product =
                    productElement.GetString() ?? "unknown";

                correlationId =
                    correlationElement.GetString() ?? "unknown";

                // OpenTelemetry tags
                activity?.SetTag(
                    "messaging.system",
                    "azure_service_bus");

                activity?.SetTag(
                    "messaging.destination",
                    "order-queue");

                activity?.SetTag(
                    "message.id",
                    args.Message.MessageId);

                activity?.SetTag(
                    "correlation.id",
                    correlationId);

                activity?.SetTag(
                    "product.name",
                    product);

                _logger.LogInformation(
                    "🟢 [PROCESSING] CorrelationId={CorrelationId} Product={Product}",
                    correlationId,
                    product);

                // Failure simulation
                if (product.Contains("FAIL"))
                {
                    throw new Exception(
                        "Simulated failure");
                }

                // Future place for idempotency
                /*
                if (AlreadyProcessed(args.Message.MessageId))
                {
                    _logger.LogWarning(
                        "Duplicate message ignored: {MessageId}",
                        args.Message.MessageId);

                    await args.CompleteMessageAsync(args.Message);

                    return;
                }
                */

                _logger.LogInformation(
                    "✅ [SUCCESS] CorrelationId={CorrelationId}",
                    correlationId);

                await args.CompleteMessageAsync(
                    args.Message);
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Invalid JSON message");

                await DeadLetterInvalidMessage(
                    args,
                    "InvalidJson",
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ [PROCESSING ERROR] CorrelationId={CorrelationId}",
                    correlationId);

                // Service Bus retries automatically
                throw;
            }
        }

        private Task HandleError(
            ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "❌ [SERVICE BUS ERROR] Entity={EntityPath} Source={ErrorSource}",
                args.EntityPath,
                args.ErrorSource);

            return Task.CompletedTask;
        }

        private async Task DeadLetterInvalidMessage(
            ProcessMessageEventArgs args,
            string reason,
            string description)
        {
            _logger.LogWarning(
                "☠️ Dead-lettering message. Reason={Reason} Description={Description}",
                reason,
                description);

            await args.DeadLetterMessageAsync(
                args.Message,
                reason,
                description);
        }

        public override async Task StopAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "🛑 Stopping Order Service...");

            if (_processor != null)
            {
                await _processor.DisposeAsync();
            }

            if (_client != null)
            {
                await _client.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}