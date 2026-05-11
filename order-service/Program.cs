using System;
using System.IO;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using order.Services;

string? appInsightsConnection = null;

// AKS mounted secret
if (File.Exists("/mnt/secrets/applicationinsights-connection"))
{
    appInsightsConnection = File
        .ReadAllText(
            "/mnt/secrets/applicationinsights-connection")
        .Trim();

    Console.WriteLine(
        "🔐 Using App Insights connection from mounted secret");
}
else
{
    // Local development fallback
    appInsightsConnection =
        Environment.GetEnvironmentVariable(
            "APPLICATIONINSIGHTS_CONNECTION_STRING");

    Console.WriteLine(
        "💻 Using App Insights connection from environment variable");
}

if (string.IsNullOrWhiteSpace(appInsightsConnection))
{
    throw new Exception(
        "Application Insights connection string not found");
}

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
                            appInsightsConnection;
                    })

                    // Console exporter for local debugging
                    .AddConsoleExporter();
            });
    });

Console.WriteLine("🚀 Starting Order Service...");

await builder.Build().RunAsync();