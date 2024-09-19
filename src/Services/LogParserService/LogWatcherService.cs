using System.Text.Json;
using Microsoft.Extensions.Logging;
using SoloHash.Worker.Models.Pool;
using SoloHash.Worker.Models.User;
using SoloHash.Worker.Services.DynamoDbService;

namespace SoloHash.Worker.Services.LogParserService;

public class LogWatcherService : ILogWatcherService
{
    private readonly ILogger<LogWatcherService> _logger;
    private readonly IDynamoDbService _dynamoDbService;
    
    private readonly FileSystemWatcher _fileSystemWatcher;

    public LogWatcherService(ILogger<LogWatcherService> logger, IDynamoDbService dynamoDbService, string directoryPath,
        string filter)
    {
        _logger = logger;
        _dynamoDbService = dynamoDbService;
        
        _fileSystemWatcher = new FileSystemWatcher();
        _fileSystemWatcher.Path = directoryPath;
        _fileSystemWatcher.Filter = filter;
        _fileSystemWatcher.NotifyFilter =
            NotifyFilters.LastWrite | NotifyFilters.FileName;
        _fileSystemWatcher.Created += (sender, e) => OnFileChanged(e.FullPath);
        _fileSystemWatcher.Changed += (sender, e) => OnFileChanged(e.FullPath);
        _fileSystemWatcher.EnableRaisingEvents = true;
    }
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private async void OnFileChanged(string filePath)
    {
        _logger.LogInformation("File changed with full path: {FilePath}", filePath);
        
        try
        {
            var fileContent = await File.ReadAllTextAsync(filePath);

            if (filePath.Contains("pool.status"))
            {
                _logger.LogInformation("Parsing pool status...");
                await ProcessPoolStatus(fileContent);
            }
            else
            {
                _logger.LogInformation("Parsing user status...");
                var filename = Path.GetFileName(filePath);
                await ProcessUserFile(filename, fileContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when reading or parsing file");
        }
    }
    
    private async Task ProcessPoolStatus(string jsonContent)
    {
        var lines = jsonContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var runtimeStatus = JsonSerializer.Deserialize<PoolStatusRuntime>(lines[0], _jsonSerializerOptions);
        var hashrateStatus = JsonSerializer.Deserialize<PoolHashrate>(lines[1], _jsonSerializerOptions);
        var statisticsStatus = JsonSerializer.Deserialize<PoolStatistics>(lines[2], _jsonSerializerOptions);
        
        await _dynamoDbService.SavePoolStatusAsync(runtimeStatus, hashrateStatus, statisticsStatus);
    }

    private async Task ProcessUserFile(string partitionKey, string jsonContent)
    {
        var userStatus = JsonSerializer.Deserialize<UserStatus>(jsonContent, _jsonSerializerOptions);
        await _dynamoDbService.SaveUserStatusAsync(partitionKey, userStatus);
        await _dynamoDbService.UpdateUserHashrateAsync(partitionKey, userStatus.Hashrate5m);
    }
}