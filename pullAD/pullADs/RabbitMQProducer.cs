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
