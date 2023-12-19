namespace Moin.Core;

/// <summary>
/// </summary>
public delegate Task MqttMessageProcessingHandler<TMessage>(IEnumerable<string> args, TMessage message);

/// <summary>
/// </summary>
public delegate bool Subscribe(string topic);

/// <summary>
/// </summary>
public interface ICustomMqttClient
{
    /// <summary>
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// </summary>
    Task Publish<TMessage>(string topic, TMessage message);

    /// <summary>
    /// </summary>
    void Subscribe<TMessage>(MqttTopicConfig<TMessage> config, MqttMessageProcessingHandler<TMessage> processingHandler) where TMessage : IMqttMessage;
}

/// <summary>
/// </summary>
public class MqttTopicConfig<TMessage>(string topic) where TMessage : IMqttMessage
{
    /// <summary>
    /// </summary>
    public string Topic { get; } = topic;
}