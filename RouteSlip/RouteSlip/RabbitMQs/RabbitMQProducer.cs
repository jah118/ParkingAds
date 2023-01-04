using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RouteSlip.util;
using Serilog;

namespace RouteSlip.RabbitMQs
{
    public class RabbitMqProducer : IMessageProducer
    {
        private readonly AppSettings _appSettings;

        public RabbitMqProducer(IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _appSettings = optionsMonitor.CurrentValue;
        }

        public void SendMessage<T>(T message)
        {
            Log.Information("Started producing");
            Console.WriteLine(message);
            // TODO USE app settings
            var factory = new ConnectionFactory
            {
                HostName = _appSettings.RabbitConn, 
                UserName = "guest",
                Password = "guest"
            };
            
            // var factory = new ConnectionFactory();
            // factory.Uri = new Uri("amqp://guest:guest@localhost:5672/");
            //
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _appSettings.RabbitQueueNameProduce,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                routingKey: _appSettings.RabbitQueueNameProduce,
                basicProperties: properties,
                body: body);
            //TODO DO better log msg 
            Log.Information(" RabbitMqProducer Sent {0}", message);
        }
    }
}