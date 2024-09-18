using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SoloHash.Worker;
using SoloHash.Worker.Factories;
using SoloHash.Worker.Options;
using SoloHash.Worker.Services.DynamoDbService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, logging) =>
    {
        // Clear default logging providers
        logging.ClearProviders();
                
        // Add Console and Debug logging
        logging.AddConsole(options =>
        {
            // Add timestamp to the console log messages
            options.FormatterName = "custom";
        });
        
        logging.AddDebug();
                
        // Optional: Configure logging level
        logging.SetMinimumLevel(LogLevel.Information);
        
        logging.AddConsoleFormatter<CustomConsoleFormatter, ConsoleFormatterOptions>();
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<LogWatcherOptions>(context.Configuration.GetSection(nameof(LogWatcherOptions)));

        services.AddScoped<IAmazonDynamoDB>(x => new AmazonDynamoDBClient(RegionEndpoint.EUWest2));
        services.AddSingleton<IDynamoDbService, DynamoDbService>();
        services.AddSingleton<LogWatcherFactory>();
        services.AddSingleton<App>();
    })
    .Build();

var app = host.Services.GetRequiredService<App>();
app.Run();

await host.WaitForShutdownAsync();