namespace SoloHash.Worker.Models.Pool;

public class PoolStatistics
{
    public double Diff { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
    public double BestShare { get; set; }
    public double SPS1m { get; set; }
    public double SPS5m { get; set; }
    public double SPS15m { get; set; }
    public double SPS1h { get; set; }
}