namespace SoloHash.Worker.Options;

public class LogWatcherOptions
{
    // User stats
    public string UserDirectoryPath { get; set; } = "/ckpool/logs/users";
    public string UserFilter { get; set; } = "*";
    
    // Pool stats
    public string PoolDirectoryPath { get; set; } = "/ckpool/logs/pool";
    public string PoolFilter { get; set; } = "*";
    
    // App (pool software) logs
    public string PoolSystemDirectoryPath { get; set; } = "/ckpool/logs";
    public string PoolSystemFilter { get; set; } = "*";
}