using Akka.Hosting.Coordination;

namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
public interface ISettings { }

/// <summary>
/// </summary>
public class ShardSettings : ISettings
{
    /// <summary>
    /// </summary>
    public int MaxShards { get; init; } = 150;

    /// <summary>
    /// </summary>
    public bool ShouldPassivateIdleEntities { get; init; } = true;

    /// <summary>
    /// </summary>
    public TimeSpan? PassivateIdleEntityAfter { get; init; } = null!;
}

/// <summary>
/// </summary>
public class SingletonSettings : ISettings
{
    /// <summary>
    /// </summary>
    public int? BufferSize { get; init; } = null;

    /// <summary>
    /// </summary>
    public LeaseOptionBase? LeaseImplementation { get; init; } = null;

    /// <summary>
    /// </summary>
    public TimeSpan? LeaseRetryInterval { get; init; } = null;
}