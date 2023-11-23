namespace FicktEuchAllee.Core;

public delegate Task MqttMessageProcessingHandler<TMessage>(IEnumerable<string> args, TMessage message);
public delegate bool Subscribe(string topic);

public interface ICustomMqttClient
{
    Task StartAsync();
    Task StopAsync();
    Task Publish<TMessage>(string topic, TMessage message);
    void Subscribe<TMessage>(MqttTopicConfig<TMessage> config, MqttMessageProcessingHandler<TMessage> processingHandler) where TMessage : IMqttMessage;
}

public class MqttTopicConfig<TMessage>(string topic) where TMessage : IMqttMessage
{
    public string Topic { get; } = topic;
}