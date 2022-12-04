using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisConsumer.Facade;

namespace RedisConsumer;

public class RabbitMQConsumer : IMessageConsumer
{
    private readonly IAppSettings _appSettings;

    public RabbitMQConsumer(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public void SendMessage<T>(T message)
    {
        throw new NotImplementedException();
    }

    public string ReceiveMessage()
    {
        var factory = new ConnectionFactory {HostName = _appSettings.RabbitConn};
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("ads");
        var consumer = new EventingBasicConsumer(channel);
        string message = null!;
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };
        channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);
        return message!;
    }
}