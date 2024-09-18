using System.Text.Json.Serialization;
using SoloHash.Worker.JsonConverters;

namespace SoloHash.Worker.Models.Pool;

public class PoolStatusRuntime
{
    public int Runtime { get; set; }
    
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime LastUpdate { get; set; }
    
    public int Users { get; set; }
    
    public int Workers { get; set; }
    
    public int Idle { get; set; }
    
    public int Disconnected { get; set; }
}