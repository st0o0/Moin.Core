using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Serilog;

namespace FicktEuchAllee.Core;

public class CustomMqttClient : ICustomMqttClient
{
    private readonly MqttFactory _factory;
    private readonly IManagedMqttClient _client;
    private readonly MqttSubscriptionHandler _handler;
    private readonly ILogger _logger = Log.Logger.ForContext<CustomMqttClient>();
    public CustomMqttClient()
    {
        _factory = new MqttFactory();
        _client = _factory.CreateManagedMqttClient();

        _handler = new(Subscribe);

        _client.ConnectedAsync += (args) =>
        {
            _logger.Debug("[MQTT] Connected [{ConnectResult}]", args.ConnectResult);
            return Task.CompletedTask;
        };
        _client.ConnectingFailedAsync += (args) =>
        {
            _logger.Debug("[MQTT] ConnectingFailed [{ConnectResult}][{Exception}]", args.ConnectResult, args.Exception);
            return Task.CompletedTask;
        };
        _client.DisconnectedAsync += (args) =>
        {
            _logger.Debug(
                "[MQTT] Disconnected [{ConnectResult}][{ClientWasConnected}][{Reason}][{Exception}]",
                args.ConnectResult, args.ClientWasConnected, args.Reason, args.Exception);

            return Task.CompletedTask;
        };
        _client.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;
    }

    public Task Publish<TMessage>(TMessage message)
    {
        return Task.CompletedTask;
    }

    public void Subscribe<TMessage>(MqttTopicConfig<TMessage> config, MqttMessageProcessingHandler<TMessage> processingHandler) where TMessage : IMqttMessage
    {
        _handler.CreateSubscription<TMessage>(config.Topic, async (msg, wildcards) => await processingHandler.Invoke(wildcards, msg));
    }

    private async Task<bool> Subscribe(string topic)
    {
        var mqttTopicFilter = _factory
        .CreateTopicFilterBuilder()
        .WithTopic(topic)
        .WithAtMostOnceQoS()
        .Build();

        await _client.SubscribeAsync(new[] { mqttTopicFilter });
        return true;
    }

    private async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        var msg = eventArgs.ApplicationMessage;
        var topic = msg.Topic;
        var payload = msg.ConvertPayloadToString();

        await _handler.HandleMessageAsync(topic, payload).ConfigureAwait(false);
    }

}
