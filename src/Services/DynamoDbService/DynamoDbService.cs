using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using SoloHash.Worker.Models;
using SoloHash.Worker.Models.Pool;
using SoloHash.Worker.Models.User;

namespace SoloHash.Worker.Services.DynamoDbService;

public class DynamoDbService(ILogger<DynamoDbService> logger, IAmazonDynamoDB dynamoDbClient) : IDynamoDbService
{
    const string TableName = "SoloHashStats";
    
    public async Task SavePoolStatusAsync(PoolStatusRuntime? runtimeStatus, PoolHashrate? hashrateStatus,
        PoolStatistics? statisticsStatus)
    {
        
        const string partitionKey = "pool";

        // Create DynamoDB Document objects for each status
        var runtimeStatusDocument = Document.FromJson(JsonSerializer.Serialize(runtimeStatus));
        var hashrateStatusDocument = Document.FromJson(JsonSerializer.Serialize(hashrateStatus));
        var statisticsStatusDocument = Document.FromJson(JsonSerializer.Serialize(statisticsStatus));

        // Create item with documents as attributes
        var item = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = partitionKey } },
            { "StatsType", new AttributeValue { S = "pool" } },
            { "RuntimeStatus", new AttributeValue { M = runtimeStatusDocument.ToAttributeMap() } },
            { "HashrateStatus", new AttributeValue { M = hashrateStatusDocument.ToAttributeMap() } },
            { "StatisticsStatus", new AttributeValue { M = statisticsStatusDocument.ToAttributeMap() } }
        };

        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };

        try
        {
            await dynamoDbClient.PutItemAsync(request);
            logger.LogInformation("Successfully saved pool status.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving pool status.");
        }
    }

    public async Task SaveUserStatusAsync(string partitionKey, UserStatus? userStatus)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = partitionKey } },
            { "StatsType", new AttributeValue { S = "user" } },
            { "UserStatus", new AttributeValue { S = JsonSerializer.Serialize(userStatus) } }
        };

        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };

        try
        {
            await dynamoDbClient.PutItemAsync(request);
            logger.LogInformation("Successfully saved user status.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving user status.");
        }
    }
}