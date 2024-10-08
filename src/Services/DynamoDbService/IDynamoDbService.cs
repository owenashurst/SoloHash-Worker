using SoloHash.Worker.Models;
using SoloHash.Worker.Models.Pool;
using SoloHash.Worker.Models.User;

namespace SoloHash.Worker.Services.DynamoDbService;

public interface IDynamoDbService
{
    /// <summary>
    /// Saves the pool status to DynamoDB
    /// </summary>
    /// <param name="poolStatusRuntime"><see cref="PoolStatusRuntime"/>The pool runtime status</param>
    /// <param name="poolHashrate"><see cref="PoolHashrate"/>The pool hashrate</param>
    /// <param name="poolStatistics"><see cref="PoolStatistics"/>The pool statistics</param>
    Task SavePoolStatusAsync(PoolStatusRuntime? poolStatusRuntime, PoolHashrate? poolHashrate, PoolStatistics? poolStatistics);

    /// <summary>
    /// Saves a user statistics to DynamoDB
    /// </summary>
    /// <param name="partitionKey">The bitcoin address, which is also the filename</param>
    /// <param name="userStatus"><see cref="UserStatus"/>The user statistics</param>
    Task SaveUserStatusAsync(string partitionKey, UserStatus? userStatus);

    /// <summary>
    /// Saves a user hashrate5m to DynamoDB
    /// </summary>
    /// <param name="partitionKey">The bitcoin address, which is also the filename</param>
    /// <param name="hashrate5m">The users hashrate5m</param>
    Task UpdateUserHashrateAsync(string partitionKey, string hashrate5m);

    /// <summary>
    /// Saves the latest block found for a user to DynamoDB
    /// </summary>
    /// <param name="partitionKey">The bitcoin address</param>
    /// <param name="block"><see cref="Block"/>The block object to save</param>
    /// <returns></returns>
    Task UpdateUserBlocksAsync(string partitionKey, Block block);
}