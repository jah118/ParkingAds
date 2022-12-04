using System.Text;
using Newtonsoft.Json;
using pullADs.Facade;
using RabbitMQ.Client;
using Serilog;

namespace pullADs;

public class RabbitMQProducer : IMessageProducer
{
    public void SendMessage<T>(T message)
    {
        Log.Information("Started producing");
        var factory = new ConnectionFactory {HostName = "localhost"};
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "ads",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "",
            routingKey: "ads",
            basicProperties: null,
            body: body);
        Log.Information(" [x] Sent {0}", message);
    }
}