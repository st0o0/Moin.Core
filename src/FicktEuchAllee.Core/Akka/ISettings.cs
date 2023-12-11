using Akka.Hosting.Coordination;

namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
public interface ISettings
{

}

public class ShardSettings : ISettings
{
    public int MaxShards { get; init; } = 150;
    public bool ShouldPassivateIdleEntities { get; init; } = true;

    public TimeSpan? PassivateIdleEntityAfter { get; init; } = null!;
}

public class SingletonSettings : ISettings
{
    public int? BufferSize { get; init; } = null;

    public LeaseOptionBase? LeaseImplementation { get; init; } = null;

    public TimeSpan? LeaseRetryInterval { get; init; } = null;
}