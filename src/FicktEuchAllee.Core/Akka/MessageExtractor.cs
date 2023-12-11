using Akka.Cluster.Sharding;

namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
/// <remarks>
/// </remarks>
public class MessageExtractor(int maxShards = 50) : HashCodeMessageExtractor(maxShards)
{
    /// <summary>
    /// </summary>
    public override string EntityId(object message)
        => message is IShardId shard ? shard.Id : Guid.NewGuid().ToString();
}
