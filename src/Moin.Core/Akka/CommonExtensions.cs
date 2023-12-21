using Akka.Actor;
using Akka.Hosting;

namespace Moin.Core;

/// <summary>
/// </summary>
public static class CommonExtensions
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="registry"></param>
    /// <returns></returns>
    /// <exception cref="MissingActorRegistryEntryException"></exception>
    public static IActorRef Get<T>(this IReadOnlyActorRegistry registry)
    {
        if (registry.TryGet<T>(out var actor))
            return actor;

        throw new MissingActorRegistryEntryException("No actor registered for key " + typeof(T).FullName);
    }

    /// <summary>
    /// </summary>
    /// <param name="registry"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="MissingActorRegistryEntryException"></exception>
    public static IActorRef Get(this IReadOnlyActorRegistry registry, Type type)
    {
        if (registry.TryGet(type, out var actor))
            return actor;

        throw new MissingActorRegistryEntryException("No actor registered for key " + type.FullName);
    }

    /// <summary>
    /// </summary>
    /// <param name="system"></param>
    /// <returns></returns>
    public static IReadOnlyActorRegistry GetRegistry(this ActorSystem system) => ActorRegistry.For(system);

    public static IActorRef GetClient(this ActorSystem system, IEndpoint endpoint)
        => system.GetRegistry().Get(endpoint.Type);

    public static IActorRef GetClient(this IUntypedActorContext context, IEndpoint endpoint)
        => context.System.GetClient(endpoint);
}
