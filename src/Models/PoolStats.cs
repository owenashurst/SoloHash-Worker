using Amazon.DynamoDBv2.DataModel;
using SoloHash.Worker.Models.Pool;

namespace SoloHash.Worker.Models;

[DynamoDBTable("SoloHashStats")]
public class PoolStats : Item
{
    [DynamoDBProperty]
    public PoolStatusRuntime PoolStatusRuntime { get; set; }

    [DynamoDBProperty]
    public PoolHashrate PoolHashrate { get; set; }

    [DynamoDBProperty]
    public PoolStatistics PoolStatistics { get; set; }
}