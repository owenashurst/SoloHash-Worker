using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SoloHash.Worker.Models;
using SoloHash.Worker.Models.Pool;
using SoloHash.Worker.Models.User;
using SoloHash.Worker.Services.DynamoDbService;

namespace SoloHash.Worker.Services.LogParserService;

public class LogWatcherService : ILogWatcherService
{
    private readonly ILogger<LogWatcherService> _logger;
    private readonly IDynamoDbService _dynamoDbService;
    
    private readonly FileSystemWatcher _fileSystemWatcher;
    
    // Used only for the PoolSystem log to keep track of the last known line read.
    // This avoids duplicating reading any block solve log entries for an address.
    private readonly Dictionary<string, long> _filePositions = new();

    public LogWatcherService(ILogger<LogWatcherService> logger, IDynamoDbService dynamoDbService, string directoryPath,
        string filter, LogWatcherType logWatcherType)
    {
        _logger = logger;
        _dynamoDbService = dynamoDbService;
        
        _fileSystemWatcher = new FileSystemWatcher();
        _fileSystemWatcher.Path = directoryPath;
        _fileSystemWatcher.Filter = filter;
        _fileSystemWatcher.NotifyFilter =
            NotifyFilters.LastWrite | NotifyFilters.FileName;
        _fileSystemWatcher.Created += (sender, e) => OnFileChanged(e.FullPath, logWatcherType);
        _fileSystemWatcher.Changed += (sender, e) => OnFileChanged(e.FullPath, logWatcherType);
        _fileSystemWatcher.EnableRaisingEvents = true;
    }
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private async void OnFileChanged(string filePath, LogWatcherType logWatcherType)
{
    _logger.LogInformation("File changed with full path: {FilePath}", filePath);

    try
    {
        if (logWatcherType == LogWatcherType.PoolSystem)
        {
            if (!_filePositions.ContainsKey(filePath))
            {
                _filePositions[filePath] = 0;
            }

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            stream.Seek(_filePositions[filePath], SeekOrigin.Begin);

            using var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                _logger.LogInformation("Processing newly added line for PoolSystem logs.");
                await ProcessPoolSystemLogs(line);
            }
            
            _filePositions[filePath] = stream.Position;
        }
        else
        {
            var fileContent = await File.ReadAllTextAsync(filePath);

            switch (logWatcherType)
            {
                case LogWatcherType.Pool:
                    _logger.LogInformation("Parsing pool status...");
                    await ProcessPoolStatus(fileContent);
                    break;

                case LogWatcherType.User:
                    _logger.LogInformation("Parsing user status...");
                    var filename = Path.GetFileName(filePath);
                    await ProcessUserFile(filename, fileContent);
                    break;
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error when updating user information. Error: {ErrorMessage}", ex.Message);
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

    private async Task ProcessPoolSystemLogs(string logLine)
    {
        var pattern = @"Block (\d+) solved by ([13a-zA-Z0-9]+)!";

        var regexMatch = Regex.Match(logLine, pattern, RegexOptions.IgnoreCase);
        if (!regexMatch.Success)
        {
            _logger.LogError("Error with RegEx match when parsing block solve log. Log: {LogMessage}", logLine);
            return;
        }
        
        var blockHeight = regexMatch.Groups[1].Value;
        var bitcoinAddress = regexMatch.Groups[2].Value;
        _logger.LogInformation($"Block solved by Bitcoin Address: {bitcoinAddress}");
        
        await _dynamoDbService.UpdateUserBlocksAsync(bitcoinAddress, new Block
        {
            BlockHeight = long.Parse(blockHeight),
            Found = DateTime.UtcNow
        });
    }
}