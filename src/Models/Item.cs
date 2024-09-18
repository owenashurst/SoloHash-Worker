using Amazon.DynamoDBv2.DataModel;

namespace SoloHash.Worker.Models;

public class Item
{
    [DynamoDBHashKey]
    public string Id { get; set; }
    
    public string StatsType { get; set; }
}