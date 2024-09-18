using System.Text.Json.Serialization;
using SoloHash.Worker.JsonConverters;

namespace SoloHash.Worker.Models.User;

public class UserStatus
{
    public string Hashrate1m { get; set; }
    
    public string Hashrate5m { get; set; }
    
    public string Hashrate1hr { get; set; }
    
    public string Hashrate1d { get; set; }
    
    public string Hashrate7d { get; set; }
    
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime LastShare { get; set; }
    
    public int Workers { get; set; }
    
    public long Shares { get; set; }
    
    public double BestShare { get; set; }
    
    public long BestEver { get; set; }
    
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime Authorised { get; set; }
    
    public Worker[] Worker { get; set; }
}