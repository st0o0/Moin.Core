using System.Reflection;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Routing;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.DependencyInjection;
using Akka.Hosting;
using Akka.Routing;

namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
public static class AkkaServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static AkkaConfigurationBuilder AddClientEndpoint(this AkkaConfigurationBuilder builder, IEndpoint endpoint)
    {
        return endpoint.EndpointType switch
        {
            EndpointType.Shard => builder.AddShardedClient(endpoint),
            EndpointType.Singleton => builder.AddSingletonClient(endpoint),
            EndpointType.ClusterRouter => builder.AddClusterRouterClient(endpoint),
            EndpointType.BroadcastRouter => builder.AddClusterRouterClient(endpoint),
            _ => throw new NotImplementedException()
        };
    }

    internal static AkkaConfigurationBuilder AddShardedClient(this AkkaConfigurationBuilder builder, IEndpoint endpoint)
    {
        InvokeGenericExtensionMethod(endpoint.Type, "WithShardRegionProxyWrap", [builder, $"{endpoint.Role}-{endpoint.Name}", endpoint.Role, new MessageExtractor(50)]);
        return builder;
    }

    internal static AkkaConfigurationBuilder AddSingletonClient(this AkkaConfigurationBuilder builder, IEndpoint endpoint)
    {
        InvokeGenericExtensionMethod(endpoint.Type, "WithSingletonProxyWrap", [builder, $"{endpoint.Role}-{endpoint.Name}", endpoint.Role]);
        return builder;
    }

    internal static AkkaConfigurationBuilder AddClusterRouterClient(this AkkaConfigurationBuilder builder, IEndpoint endpoint)
    {
        return builder.WithActors((system, registry) =>
        {
            registry.TryRegister(endpoint.Type, system.CreateClusterRouter($"{endpoint.Role}-{endpoint.Name}", endpoint.Role));
        });
    }

    internal static AkkaConfigurationBuilder AddBroadcastRouterCluster(this AkkaConfigurationBuilder builder, IEndpoint endpoint)
    {
        return builder.WithActors((system, registry) =>
        {
            registry.TryRegister(endpoint.Type, system.CreateBroadcastRouter($"{endpoint.Role}-{endpoint.Name}", endpoint.Role));
        });
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="endpoint"></param>
    /// <param name="settings"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static AkkaConfigurationBuilder AddServiceEndpoint<TImplementation>(this AkkaConfigurationBuilder builder, IEndpoint endpoint, ISettings? settings = default, params object[] args) where TImplementation : ActorBase
    {
        return endpoint.EndpointType switch
        {
            EndpointType.Shard => builder.AddShardedService<TImplementation>(endpoint.Name, endpoint.Role, (settings as ShardSettings) ?? new ShardSettings(), args),
            EndpointType.Singleton => builder.AddSingletonService<TImplementation>(endpoint.Name, endpoint.Role, (settings as SingletonSettings) ?? new SingletonSettings(), args),
            EndpointType.ClusterRouter or EndpointType.BroadcastRouter => builder.AddRouterService<TImplementation>(endpoint.Name, endpoint.Role, args),
            _ => throw new NotImplementedException()
        };
    }

    internal static AkkaConfigurationBuilder AddShardedService<TImplementation>(this AkkaConfigurationBuilder builder, string name, string role, ShardSettings settings, params object[] args) where TImplementation : ActorBase
    {
        builder.WithShardRegion<TImplementation>($"{role}-{name}",
        (_, _, dependecyResolver) => entityId => dependecyResolver.Props<TImplementation>(entityId),
        new MessageExtractor(settings.MaxShards),
        new()
        {
            Role = role,
            ShouldPassivateIdleEntities = settings.ShouldPassivateIdleEntities,
            PassivateIdleEntityAfter = settings.PassivateIdleEntityAfter,
            JournalOptions = settings.JournalOptions,
            SnapshotOptions = settings.SnapshotOptions
        });

        return builder;
    }

    internal static AkkaConfigurationBuilder AddSingletonService<TImplementation>(this AkkaConfigurationBuilder builder, string name, string role, SingletonSettings settings, params object[] args) where TImplementation : ActorBase
    {
        builder.WithSingleton<TImplementation>(name,
        (_, _, dependecyResolver) => dependecyResolver.Props<TImplementation>(),
        new ClusterSingletonOptions()
        {
            Role = role,
            BufferSize = settings.BufferSize,
            LeaseImplementation = settings.LeaseImplementation,
            LeaseRetryInterval = settings.LeaseRetryInterval
        },
        false);

        return builder;
    }

    internal static AkkaConfigurationBuilder AddRouterService<TImplementation>(this AkkaConfigurationBuilder builder, string name, string role, params object[] args) where TImplementation : ActorBase
    {
        builder.WithActors((system, registry) =>
        {
            var actor = system.ResolveActor<TImplementation>($"{role}-{name}");
            registry.Register<TImplementation>(actor);
        });

        return builder;
    }

    internal static IActorRef ResolveActor<T>(this ActorSystem system, string name, params object[] args) where T : ActorBase
    {
        var props = system.CreateProps<T>(args);
        return system.ActorOf(props, name);
    }

    internal static Props CreateProps<T>(this ActorSystem system, params object[] args) where T : ActorBase => DependencyResolver.For(system).Props<T>(args);

    internal static IActorRef CreateClusterRouter(this ActorSystem system, string actorName, string role, ConsistentHashMapping? hashMapping = null)
    {
        return system.ActorOf(
            Props.Empty.WithRouter(new ClusterRouterGroup(
                new ConsistentHashingGroup(["/user/" + actorName], hashMapping ??= msg => msg),
                new ClusterRouterGroupSettings(1000, ["/user/" + actorName], true, useRole: role))),
                $"{actorName}Router");
    }

    internal static IActorRef CreateBroadcastRouter(this ActorSystem system, string actorname, string role)
    {
        return system.ActorOf(
            Props.Empty.WithRouter(new ClusterRouterGroup(
                new BroadcastGroup(["/user/" + actorname]),
                new ClusterRouterGroupSettings(1000, ["/user/" + actorname], true, role))),
            $"{actorname}Router"
        );
    }

    private static void InvokeGenericExtensionMethod(Type type, string methodName, params object[] args)
    {
        var method = typeof(AkkaServiceCollectionExtensions).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)
                            ?? throw new InvalidOperationException($"Failed to InvokeGenericExtensionMethod [{methodName}]");
        var genericMethod = method.MakeGenericMethod(type);
        genericMethod.Invoke(null, args);
    }

    internal static void WithShardRegionProxyWrap<T>(AkkaConfigurationBuilder builder, string name, string role, IMessageExtractor extractor)
        => builder.WithShardRegionProxy<T>(name, role, extractor);

    internal static void WithSingletonProxyWrap<T>(AkkaConfigurationBuilder builder, string name, string role)
        => builder.WithActors((system, registry) =>
        {
            var singletonProxySettings = ClusterSingletonProxySettings.Create(system).WithSingletonName(name).WithRole(role);
            var singletonProxyProps = ClusterSingletonProxy.Props($"/user/{name}", singletonProxySettings);
            registry.Register<T>(system.ActorOf(singletonProxyProps, $"{name}-proxy"));
        });
}
