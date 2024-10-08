using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using SoloHash.Worker.Models;
using SoloHash.Worker.Models.Pool;
using SoloHash.Worker.Models.User;

namespace SoloHash.Worker.Services.DynamoDbService;

public class DynamoDbService(ILogger<DynamoDbService> logger, IAmazonDynamoDB dynamoDbClient) : IDynamoDbService
{
    public async Task SavePoolStatusAsync(PoolStatusRuntime? poolStatusRuntime, PoolHashrate? poolHashrate,
        PoolStatistics? poolStatistics)
    {
        using DynamoDBContext context = new DynamoDBContext(dynamoDbClient);

        var poolStats = new PoolStats
        {
            Id = "pool",
            StatsType = StatsType.Pool,
            PoolStatusRuntime = poolStatusRuntime,
            PoolHashrate = poolHashrate,
            PoolStatistics = poolStatistics
        };

        try
        {
            await context.SaveAsync(poolStats);
            logger.LogInformation("Successfully saved pool status.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving pool status. Error: {ErrorMessage}", ex.Message);
        }
    }

    public async Task SaveUserStatusAsync(string partitionKey, UserStatus? userStatus)
    {
        using DynamoDBContext context = new DynamoDBContext(dynamoDbClient);

        var userStats = new UserStats
        {
            Id = partitionKey,
            StatsType = StatsType.User,
            UserStatus = userStatus
        };

        try
        {
            await context.SaveAsync(userStats);
            logger.LogInformation("Successfully saved user status.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving user status. Error: {ErrorMessage}", ex.Message);
        }
    }
    
    public async Task UpdateUserHashrateAsync(string partitionKey, string hashrate5m)
    {
        using DynamoDBContext context = new DynamoDBContext(dynamoDbClient);

        var userStats = new UserHashrateStats
        {
            Id = partitionKey,
            EntryTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Hashrate5m = hashrate5m,
            ExpiryDate = SetExpiryTime(24)
        };

        try
        {
            await context.SaveAsync(userStats);
            logger.LogInformation("Successfully saved user hashrate stats.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving user hashrate stats. Error: {ErrorMessage}", ex.Message);
        }
    }
    
    public async Task UpdateUserBlocksAsync(string partitionKey, Block block)
    {
        using DynamoDBContext context = new DynamoDBContext(dynamoDbClient);
        
        UserBlockStats? userBlockStats = await context.LoadAsync<UserBlockStats?>(partitionKey, StatsType.UserBlock);
        
        if (userBlockStats is null)
        {
            userBlockStats = new UserBlockStats
            {
                Id = partitionKey,
                StatsType = StatsType.UserBlock,
                Blocks = new List<Block> { block }
            };
        }
        
        userBlockStats.Blocks.Add(block);

        try
        {
            await context.SaveAsync(userBlockStats);
            logger.LogInformation("Successfully saved user blocks stats.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when saving user block stats. Error: {ErrorMessage}", ex.Message);
        }
    }
    
    private long SetExpiryTime(int hoursToLive)
    {
        return DateTimeOffset.UtcNow.AddHours(hoursToLive).ToUnixTimeSeconds();
    }
}