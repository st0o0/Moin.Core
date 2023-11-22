namespace FicktEuchAllee.Core;

public delegate Task MqttMessageProcessingHandler<TMessage>(IEnumerable<string> args, TMessage message);
public delegate Task<bool> Subscribe(string topic);

public interface ICustomMqttClient
{
    Task Publish<TMessage>(TMessage message);

    void Subscribe<TMessage>(MqttTopicConfig<TMessage> config, MqttMessageProcessingHandler<TMessage> processingHandler) where TMessage : IMqttMessage;
}

public class MqttTopicConfig<TMessage>(string topic) where TMessage : IMqttMessage
{
    public string Topic { get; } = topic;

    public Type MessageType => typeof(TMessage);
}