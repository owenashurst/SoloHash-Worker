using Amazon.DynamoDBv2.DataModel;

namespace SoloHash.Worker.Models;

public class HashrateItem
{
    [DynamoDBHashKey]
    public string Id { get; set; }
}