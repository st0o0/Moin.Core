using System.Text.Json;

namespace FicktEuchAllee.Core;

public interface IMqttMessage
{ }

public abstract class MqttSubscription(string topic)
{
    public string Topic { get; } = topic;
    public abstract Task ExecuteIfMatch(string topic, string payload);
}

public class MqttSubscription<TMessage>(string topic, Func<TMessage, IEnumerable<string>, Task> callback) : MqttSubscription(topic) where TMessage : IMqttMessage
{
    public override async Task ExecuteIfMatch(string topic, string payload)
    {
        if (!Matcher.Match(Topic, topic, out var wildcards)) return;
        var payloadObject = JsonSerializer.Deserialize<TMessage>(payload);
        await callback.Invoke(payloadObject!, wildcards);
    }
}

public class MqttSubscriptionHandler(Subscribe subscribe)
{
    private readonly List<MqttSubscription> _subscriptions = [];
    private readonly HashSet<string> _localSubscriber = [];
    private readonly Subscribe _subscribe = subscribe;

    public bool HasTopicSubscribed(string topic) => _localSubscriber.Contains(topic);

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

    public bool CreateSubscription<TMessage>(string topic, Func<TMessage, IEnumerable<string>, Task> callback) where TMessage : IMqttMessage
    {
        if (_localSubscriber.Contains(topic)) return false;
        _localSubscriber.Add(topic);

        _subscribe.Invoke(topic);
        _subscriptions.Add(new MqttSubscription<TMessage>(topic, callback));
        return true;
    }
}