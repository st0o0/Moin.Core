using System.Text.Json;

namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
public interface IMqttMessage
{ }

/// <summary>
/// </summary>
/// <param name="topic"></param>
public abstract class MqttSubscription(string topic)
{
    /// <summary>
    /// </summary>
    public string Topic { get; } = topic;

    /// <summary>
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    public abstract Task ExecuteIfMatch(string topic, string payload);
}

/// <summary>
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <param name="topic"></param>
/// <param name="callback"></param>
public class MqttSubscription<TMessage>(string topic, Func<TMessage, IEnumerable<string>, Task> callback) : MqttSubscription(topic) where TMessage : IMqttMessage
{
    /// <summary>
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    public override async Task ExecuteIfMatch(string topic, string payload)
    {
        if (!Matcher.Match(Topic, topic, out var wildcards)) return;
        var payloadObject = JsonSerializer.Deserialize<TMessage>(payload);
        await callback.Invoke(payloadObject!, wildcards);
    }
}

/// <summary>
/// </summary>
/// <param name="subscribe"></param>
public class MqttSubscriptionHandler(Subscribe subscribe)
{
    private readonly List<MqttSubscription> _subscriptions = [];
    private readonly HashSet<string> _localSubscriber = [];
    private readonly Subscribe _subscribe = subscribe;

    /// <summary>
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public bool HasTopicSubscribed(string topic) => _localSubscriber.Contains(topic);

    /// <summary>
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    public async Task HandleMessageAsync(string topic, string payload)
    {
        try
        {
            foreach (var item in _subscriptions)
            {
                await item.ExecuteIfMatch(topic, payload).ConfigureAwait(false);
            }

        }
        catch (Exception)
        {
            // todo: logging;
            throw;
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="topic"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public bool CreateSubscription<TMessage>(string topic, Func<TMessage, IEnumerable<string>, Task> callback) where TMessage : IMqttMessage
    {
        if (_localSubscriber.Contains(topic)) return false;
        _localSubscriber.Add(topic);

        _subscribe.Invoke(topic);
        _subscriptions.Add(new MqttSubscription<TMessage>(topic, callback));
        return true;
    }
}