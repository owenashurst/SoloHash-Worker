using Microsoft.Extensions.Logging;
using SoloHash.Worker.Factories;

namespace SoloHash.Worker;

public class App(ILogger<App> logger, LogWatcherFactory logWatcherFactory)
{
    public void Run()
    {
        var logWatcherTypes = new List<LogWatcherType> 
        {
            LogWatcherType.User,
            LogWatcherType.Pool
        };

        foreach (var type in logWatcherTypes)
        {
            var service = logWatcherFactory.Create(type);
            service.StartWatching();
            
            logger.LogInformation("Started watching directory for type '{Type}'", Enum.GetName(type));
        }
    }
}