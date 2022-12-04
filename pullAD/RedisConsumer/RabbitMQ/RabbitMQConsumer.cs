using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisConsumer.Facade;
using Serilog;
using System.Threading;


namespace RedisConsumer.RabbitMQ;

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

        channel.QueueDeclare("ads",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
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
            channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        };
        channel.BasicConsume(queue: "orders",
            autoAck: false,
            consumer: consumer);
        return message!;
    }
}