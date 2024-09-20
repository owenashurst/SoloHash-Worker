namespace SoloHash.Worker.Models.Pool;

public class PoolStatistics
{
    public double Diff { get; set; }
    public long Accepted { get; set; }
    public long Rejected { get; set; }
    public decimal BestShare { get; set; }
    public double SPS1m { get; set; }
    public double SPS5m { get; set; }
    public double SPS15m { get; set; }
    public double SPS1h { get; set; }
}