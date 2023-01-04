using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ParkingAdsAPI.Util;
using RabbitMQ.Client;
using Serilog;
using System.Text;

namespace ParkingAdsAPI.RabbitMQs
{
    public class RabbitMQProducer : IMessageProducer
    {
        private readonly AppSettings _appSettings;

        public RabbitMQProducer(IOptionsMonitor<AppSettings> optionsMonitor)
        {
            //_appSettings = new AppSettings();
            //config.GetSection("ApiSettings").Bind(_appSettings);
            _appSettings = optionsMonitor.CurrentValue;

        }
        public void SendMessage<T>(T message)
        {
            Log.Information("Started producing");
            Console.WriteLine(message);
            // TODO USE app settings
            var factory = new ConnectionFactory { HostName = _appSettings.RabbitConn };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _appSettings.RabbitQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                routingKey: _appSettings.RabbitQueueName,
                basicProperties: properties,
                body: body);
            //TODO DO better log msg 
            Log.Information(" [x] Sent {0}", message);
        }
    }
}
