namespace SoloHash.Worker.Models.Pool;

public class PoolStatusRuntime
{
    public int Runtime { get; set; }
    public int LastUpdate { get; set; }
    public int Users { get; set; }
    public int Workers { get; set; }
    public int Idle { get; set; }
    public int Disconnected { get; set; }
}