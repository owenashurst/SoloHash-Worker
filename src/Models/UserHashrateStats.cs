using Amazon.DynamoDBv2.DataModel;
using SoloHash.Worker.Models.User;

namespace SoloHash.Worker.Models;

[DynamoDBTable("SoloHashUserHashrateStats")]
public class UserHashrateStats : HashrateItem
{
    [DynamoDBProperty]
    public string Hashrate5m { get; set; }
    
    [DynamoDBProperty] 
    public long ExpiryDate { get; set; }
}