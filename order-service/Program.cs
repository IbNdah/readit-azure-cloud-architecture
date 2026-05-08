using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.Exporter;
using order.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<OrderConsumer>();

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService("order-service"))

                    .AddSource("readit.order")

                    // Azure Application Insights Exporter
                    .AddAzureMonitorTraceExporter(options =>
                    {
                        options.ConnectionString =
                            Environment.GetEnvironmentVariable(
                                "APPLICATIONINSIGHTS_CONNECTION_STRING");
                    })

                    // Console exporter for local debugging
                    .AddConsoleExporter();
            });
    });

Console.WriteLine("🚀 Starting Order Service...");

await builder.Build().RunAsync();