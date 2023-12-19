using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Serilog;

namespace Moin.Core;

/// <summary>
/// </summary>
public class CustomMqttClient : ICustomMqttClient
{
    private readonly MqttFactory _factory;
    private readonly IManagedMqttClient _client;
    private readonly MqttSubscriptionHandler _handler;
    private readonly ILogger _logger = Log.Logger.ForContext<CustomMqttClient>();
    private readonly MqttConfiguration _conifg;

    /// <summary>
    /// </summary>
    public CustomMqttClient(MqttConfiguration configuration)
    {
        _conifg = configuration;
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

    /// <summary>
    /// </summary>
    public async Task StartAsync()
    {
        var clientOptions = _factory
        .CreateClientOptionsBuilder()
        .WithTcpServer(_conifg.Host, _conifg.Port)
        .WithCredentials(_conifg.Username, _conifg.Password)
        .WithClientId(Guid.NewGuid().ToString())
        .WithProtocolVersion(MqttProtocolVersion.V500)
        .Build();

        var options = _factory
        .CreateManagedMqttClientOptionsBuilder()
        .WithPendingMessagesOverflowStrategy(MqttPendingMessagesOverflowStrategy.DropNewMessage)
        .WithMaxPendingMessages(250)
        .WithClientOptions(clientOptions)
        .Build();

        await _client.StartAsync(options);
    }

    /// <summary>
    /// </summary>
    public async Task StopAsync() => await _client.StopAsync();

    /// <summary>
    /// </summary>
    public async Task Publish<TMessage>(string topic, TMessage message)
    {
        var payload = JsonSerializer.Serialize(message);

        var applicationMessage = _factory
        .CreateApplicationMessageBuilder()
        .WithTopic(topic)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithPayload(payload)
        .Build();

        await _client.EnqueueAsync(applicationMessage);
    }

    /// <summary>
    /// </summary>
    public void Subscribe<TMessage>(MqttTopicConfig<TMessage> config, MqttMessageProcessingHandler<TMessage> processingHandler) where TMessage : IMqttMessage
    {
        _handler.CreateSubscription<TMessage>(config.Topic, async (msg, wildcards) => await processingHandler.Invoke(wildcards, msg));
    }

    private bool Subscribe(string topic)
    {
        var mqttTopicFilter = _factory
        .CreateTopicFilterBuilder()
        .WithTopic(topic)
        .WithAtLeastOnceQoS()
        .Build();

        Task.Run(async () => await _client.SubscribeAsync(new[] { mqttTopicFilter }).ConfigureAwait(false));
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
