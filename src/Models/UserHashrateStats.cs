using Amazon.DynamoDBv2.DataModel;
using SoloHash.Worker.Models.User;

namespace SoloHash.Worker.Models;

[DynamoDBTable("SoloHashStats")]
public class UserHashrateStats : Item
{
    [DynamoDBProperty]
    public string Hashrate5m { get; set; }
    
    [DynamoDBProperty] 
    public long ExpiryTime  => SetExpiryTime(24);
    
    private long SetExpiryTime(int hoursToLive)
    {
        return DateTimeOffset.UtcNow.AddHours(hoursToLive).ToUnixTimeSeconds();
    }
}