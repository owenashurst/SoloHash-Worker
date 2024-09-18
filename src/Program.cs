using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoloHash.Worker;
using SoloHash.Worker.Factories;
using SoloHash.Worker.Services.DynamoDbService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, logging) =>
    {
        // Clear default logging providers
        logging.ClearProviders();
                
        // Add Console and Debug logging
        logging.AddConsole();
        logging.AddDebug();
                
        // Optional: Configure logging level
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IAmazonDynamoDB>();
        services.AddSingleton<IDynamoDbService, DynamoDbService>();
        services.AddSingleton<LogWatcherFactory>();
        services.AddSingleton<App>();
    })
    .Build();

var app = host.Services.GetRequiredService<App>();
app.Run();

await host.WaitForShutdownAsync();