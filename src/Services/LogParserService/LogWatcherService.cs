using System.Text.Json;
using Microsoft.Extensions.Logging;
using SoloHash.Worker.Models;
using SoloHash.Worker.Models.Pool;
using SoloHash.Worker.Models.User;
using SoloHash.Worker.Services.DynamoDbService;

namespace SoloHash.Worker.Services.LogParserService;

public class LogWatcherService(ILogger<LogWatcherService> logger, IDynamoDbService dynamoDbService, string directoryPath, string filter) : ILogWatcherService
{
    private JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    
    public void StartWatching()
    {
        var watcher = new FileSystemWatcher
        {
            Path = directoryPath,
            Filter = filter,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        watcher.Created += (sender, e) => OnFileChanged(e.FullPath);
        watcher.Changed += (sender, e) => OnFileChanged(e.FullPath);
        watcher.EnableRaisingEvents = true;
    }
    
    private async void OnFileChanged(string filePath)
    {
        logger.LogInformation("File changed with full path: {FilePath}", filePath);
        
        try
        {
            var fileContent = await File.ReadAllTextAsync(filePath);

            if (filePath.Contains("pool.status"))
            {
                logger.LogInformation("Parsing pool status...");
                await ProcessPoolStatus(fileContent);
            }
            else
            {
                logger.LogInformation("Parsing user status...");
                var filename = Path.GetFileName(filePath);
                await ProcessUserFile(filename, fileContent);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when reading or parsing file");
        }
    }
    
    private async Task ProcessPoolStatus(string jsonContent)
    {
        var lines = jsonContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var runtimeStatus = JsonSerializer.Deserialize<PoolStatusRuntime>(lines[0], JsonSerializerOptions);
        var hashrateStatus = JsonSerializer.Deserialize<PoolHashrate>(lines[1], JsonSerializerOptions);
        var statisticsStatus = JsonSerializer.Deserialize<PoolStatistics>(lines[2], JsonSerializerOptions);
        
        await dynamoDbService.SavePoolStatusAsync(runtimeStatus, hashrateStatus, statisticsStatus);
    }

    private async Task ProcessUserFile(string partitionKey, string jsonContent)
    {
        var userStatus = JsonSerializer.Deserialize<UserStatus>(jsonContent, JsonSerializerOptions);
        await dynamoDbService.SaveUserStatusAsync(partitionKey, userStatus);
    }
}