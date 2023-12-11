namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// </summary>
    string Role { get; }

    /// <summary>
    /// </summary>
    EndpointType EndpointType { get; }

    /// <summary>
    /// </summary>
    Type Type { get; }
}

/// <summary>
/// </summary>
public record Endpoint<T>(string Name, string Role, EndpointType EndpointType) : IEndpoint where T : notnull
{
    /// <summary>
    /// </summary>
    public Type Type => typeof(T);
}

/// <summary>
/// </summary>
public static class EndpointBuilder
{
    /// <summary>
    /// </summary>
    public static Endpoint<T> Create<T>(string name, string role, EndpointType type) where T : notnull
        => new(name, role, type);
}

/// <summary>
/// </summary>
public enum EndpointType
{
    /// <summary>
    /// </summary>
    Shard,

    /// <summary>
    /// </summary>
    Singleton,

    /// <summary>
    /// </summary>
    ClusterRouter,

    /// <summary>
    /// </summary>
    BroadcastRouter
}