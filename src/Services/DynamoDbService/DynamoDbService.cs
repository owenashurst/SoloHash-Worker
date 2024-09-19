using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
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
            StatsType = "pool",
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
            logger.LogError(ex, "Error saving pool status.");
        }
    }

    public async Task SaveUserStatusAsync(string partitionKey, UserStatus? userStatus)
    {
        using DynamoDBContext context = new DynamoDBContext(dynamoDbClient);

        var userStats = new UserStats
        {
            Id = partitionKey,
            StatsType = "user",
            UserStatus = userStatus
        };

        try
        {
            await context.SaveAsync(userStats);
            logger.LogInformation("Successfully saved user status.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving user status.");
        }
    }
    
    public async Task UpdateUserHashrateAsync(string partitionKey, string hashrate5m)
    {
        using DynamoDBContext context = new DynamoDBContext(dynamoDbClient);

        var userStats = new UserHashrateStats
        {
            Id = partitionKey,
            Hashrate5m = hashrate5m,
            ExpiryTime = SetExpiryTime(24)
        };

        try
        {
            await context.SaveAsync(userStats);
            logger.LogInformation("Successfully saved user hashrate stats.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving user hashrate stats.");
        }
    }
    
    private long SetExpiryTime(int hoursToLive)
    {
        return DateTimeOffset.UtcNow.AddHours(hoursToLive).ToUnixTimeSeconds();
    }
}