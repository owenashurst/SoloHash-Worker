using Microsoft.Extensions.Logging;
using SoloHash.Worker.Options;
using SoloHash.Worker.Services.DynamoDbService;
using SoloHash.Worker.Services.LogParserService;

namespace SoloHash.Worker.Factories;

public class LogWatcherFactory(ILogger<LogWatcherService> logger, IDynamoDbService dynamoDbService, LogWatcherOptions logWatcherOptions)
{
    public ILogWatcherService Create(LogWatcherType type)
    {
        return type switch
        {
            LogWatcherType.User => new LogWatcherService(
                logger,
                dynamoDbService,
                logWatcherOptions.UserDirectoryPath,
                logWatcherOptions.UserFilter),
            LogWatcherType.Pool => new LogWatcherService(
                logger,
                dynamoDbService,
                logWatcherOptions.PoolDirectoryPath,
                logWatcherOptions.PoolFilter),
            _ => throw new ArgumentException("Invalid LogWatcherType", nameof(type))
        };
    }
}