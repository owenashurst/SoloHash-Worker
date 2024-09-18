namespace SoloHash.Worker.Options;

public class LogWatcherOptions
{
    public string UserDirectoryPath { get; set; } = "/ckpool/logs/users";
    public string UserFilter { get; set; } = "*";
    public string PoolDirectoryPath { get; set; } = "/ckpool/logs/pool";
    public string PoolFilter { get; set; } = "*";
}