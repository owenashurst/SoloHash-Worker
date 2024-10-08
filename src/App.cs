using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoloHash.Worker.Options;
using SoloHash.Worker.Services.DynamoDbService;
using SoloHash.Worker.Services.LogParserService;

namespace SoloHash.Worker;

public class App(ILogger<App> logger, ILogger<LogWatcherService> loggerWatcherService, IDynamoDbService dynamoDbService, IOptions<LogWatcherOptions> logWatcherOptions)
{
    private List<LogWatcherService> _logWatcherServices = [];
    
    public void Run()
    {
        var directoriesToWatch = new List<(string path, string filter, LogWatcherType logWatcherType)>
        {
            (logWatcherOptions.Value.PoolDirectoryPath, logWatcherOptions.Value.PoolFilter, LogWatcherType.Pool),
            (logWatcherOptions.Value.UserDirectoryPath, logWatcherOptions.Value.UserFilter, LogWatcherType.User),
            (logWatcherOptions.Value.PoolDirectoryPath, logWatcherOptions.Value.PoolSystemFilter, LogWatcherType.PoolSystem)
        };
        
        foreach (var (path, filter, logWatcherType) in directoriesToWatch)
        {
            var watcherService = new LogWatcherService(loggerWatcherService, dynamoDbService, path, filter, logWatcherType);
            _logWatcherServices.Add(watcherService);
            logger.LogInformation("Started watching directory '{FileDirectory}' for type '{Type}'", path, Enum.GetName(logWatcherType));
        }
    }
}