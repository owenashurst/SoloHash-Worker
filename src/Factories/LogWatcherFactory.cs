using Microsoft.Extensions.Logging;
using SoloHash.Worker.Services.DynamoDbService;
using SoloHash.Worker.Services.LogParserService;

namespace SoloHash.Worker.Factories;

public class LogWatcherFactory(ILogger<LogWatcherService> logger, IDynamoDbService dynamoDbService)
{
    public ILogWatcherService Create(LogWatcherType type)
    {
        return type switch
        {
            LogWatcherType.User => new LogWatcherService(
                logger,
                dynamoDbService,
                "/home/ckpool/logs/users",
                "*"),
            LogWatcherType.Pool => new LogWatcherService(
                logger,
                dynamoDbService,
                "/home/ckpool/logs/pool",
                "*"),
            _ => throw new ArgumentException("Invalid LogWatcherType", nameof(type))
        };
    }
}