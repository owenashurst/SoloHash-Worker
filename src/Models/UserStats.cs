using Amazon.DynamoDBv2.DataModel;
using SoloHash.Worker.Models.User;

namespace SoloHash.Worker.Models;

[DynamoDBTable("SoloHashStats")]
public class UserStats : StatsItem
{
    [DynamoDBProperty]
    public UserStatus UserStatus { get; set; }
}