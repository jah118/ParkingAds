using System.Text;
using Newtonsoft.Json;
using pullADs.Facade;
using RabbitMQ.Client;
using Serilog;
using System.Threading;

namespace pullADs;

public class RabbitMQProducer : IMessageProducer
{
    private readonly IAppSettings _appSettings;

    public RabbitMQProducer(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public void SendMessage<T>(T message)
    {
        Log.Information("Started producing");
        // TODO USE app settings
        var factory = new ConnectionFactory {HostName = _appSettings.RabbitConn};
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "ads",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);
        
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        
        channel.BasicPublish(exchange: "",
            routingKey: "ads",
            basicProperties: properties,
            body: body);
        //TODO DO better log msg 
        Log.Information(" [x] Sent {0}", message);
    }
}


/// from https://github.com/dotnet-architecture/eShopOnContainers/blob/main/src/BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.cs
///         public void Publish(IntegrationEvent @event)
// {
// if (!_persistentConnection.IsConnected)
// {
//     _persistentConnection.TryConnect();
// }
//
// var policy = RetryPolicy.Handle<BrokerUnreachableException>()
//     .Or<SocketException>()
//     .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
//     {
//         _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
//     });
//
// var eventName = @event.GetType().Name;
//
// _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);
//
// using (var channel = _persistentConnection.CreateModel())
// {
//     _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
//
//     channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
//
//     var message = JsonConvert.SerializeObject(@event);
//     var body = Encoding.UTF8.GetBytes(message);
//
//     policy.Execute(() =>
//     {
//         var properties = channel.CreateBasicProperties();
//         properties.DeliveryMode = 2; // persistent
//
//         _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);
//
//         channel.BasicPublish(
//             exchange: BROKER_NAME,
//             routingKey: eventName,
//             mandatory: true,
//             basicProperties: properties,
//             body: body);
//     });
// }
// }
/// 