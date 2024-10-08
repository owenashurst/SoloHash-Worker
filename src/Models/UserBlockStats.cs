using Amazon.DynamoDBv2.DataModel;

namespace SoloHash.Worker.Models;

[DynamoDBTable("SoloHashStats")]
public class UserBlockStats : StatsItem
{
    [DynamoDBProperty] 
    public IList<Block> Blocks { get; set; }
}