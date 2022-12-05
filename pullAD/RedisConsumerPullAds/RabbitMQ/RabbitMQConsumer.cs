using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisConsumerPullAds.Facade;
using Serilog;

namespace RedisConsumerPullAds.RabbitMQ;

public class RabbitMQConsumer : IMessageConsumer
{
    private readonly IAppSettings _appSettings;

    public RabbitMQConsumer(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public string ReceiveMessage()
    {
        var factory = new ConnectionFactory {HostName = _appSettings.RabbitConn};
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(_appSettings.RabbitChannelRedisAds,
            true,
            false,
            false,
            null);

        channel.BasicQos(0, 1, false);
        Log.Information(" [*] Waiting for messages.");


        var consumer = new EventingBasicConsumer(channel);
        string message = null!;
        consumer.Received += (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            message = Encoding.UTF8.GetString(body);
            Log.Information(" [x] Received {message}", message);


            // Note: it is possible to access the channel via
            //       ((EventingBasicConsumer)sender).Model here
            channel.BasicAck(eventArgs.DeliveryTag, false);
        };
        channel.BasicConsume(_appSettings.RabbitChannelRedisAds,
            false,
            consumer);
        return message!;
    }
}