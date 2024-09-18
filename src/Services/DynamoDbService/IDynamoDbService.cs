using SoloHash.Worker.Models;
using SoloHash.Worker.Models.Pool;
using SoloHash.Worker.Models.User;

namespace SoloHash.Worker.Services.DynamoDbService;

public interface IDynamoDbService
{
    /// <summary>
    /// Saves the pool status to DynamoDB
    /// </summary>
    /// <param name="runtimeStatus"><see cref="PoolStatusRuntime"/>The pool runtime status</param>
    /// <param name="hashrateStatus"><see cref="PoolHashrate"/>The pool hashrate</param>
    /// <param name="statisticsStatus"><see cref="PoolStatistics"/>The pool statistics</param>
    Task SavePoolStatusAsync(PoolStatusRuntime? runtimeStatus, PoolHashrate? hashrateStatus, PoolStatistics? statisticsStatus);

    /// <summary>
    /// Saves a user statistics to DynamoDB
    /// </summary>
    /// <param name="partitionKey">The bitcoin address, which is also the filename</param>
    /// <param name="userStatus"><see cref="UserStatus"/>The user statistics</param>
    Task SaveUserStatusAsync(string partitionKey, UserStatus? userStatus);
}