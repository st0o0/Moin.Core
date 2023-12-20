using Akka.Cluster.Sharding;

namespace Moin.Core;

/// <summary>
/// 
/// </summary>
/// <param name="maxShards"></param>
public class MessageExtractor(int maxShards = 50) : HashCodeMessageExtractor(maxShards)
{
    /// <summary>
    /// </summary>
    public override string EntityId(object message)
        => message is IEntityId ntt ? ntt.EntityId : throw new InvalidOperationException("no entityId");
}
