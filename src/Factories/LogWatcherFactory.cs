using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoloHash.Worker.Options;
using SoloHash.Worker.Services.DynamoDbService;
using SoloHash.Worker.Services.LogParserService;

namespace SoloHash.Worker.Factories;

public class LogWatcherFactory(ILogger<LogWatcherService> logger, IDynamoDbService dynamoDbService, IOptions<LogWatcherOptions> logWatcherOptions)
{
    public ILogWatcherService Create(LogWatcherType type)
    {
        return type switch
        {
            LogWatcherType.User => new LogWatcherService(
                logger,
                dynamoDbService,
                logWatcherOptions.Value.UserDirectoryPath,
                logWatcherOptions.Value.UserFilter),
            LogWatcherType.Pool => new LogWatcherService(
                logger,
                dynamoDbService,
                logWatcherOptions.Value.PoolDirectoryPath,
                logWatcherOptions.Value.PoolFilter),
            _ => throw new ArgumentException("Invalid LogWatcherType", nameof(type))
        };
    }
}