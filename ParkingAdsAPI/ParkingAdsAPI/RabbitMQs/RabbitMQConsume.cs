using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ParkingAdsAPI.Data;
using ParkingAdsAPI.Util;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace ParkingAdsAPI.RabbitMQs;

public class RabbitMQConsume : IMessageConsume
{
    private readonly AppSettings _appSettings;
    private bool go = true;

    public RabbitMQConsume(IOptionsMonitor<AppSettings> optionsMonitor)
    {
        _appSettings = optionsMonitor.CurrentValue;
    }

    // public async Task<JsonModel> GetMessages()
    // {
    //     GetMessage();
    //     HoldMsg();
    //
    // }

    // public async Task<JsonModel> HoldMsg()
    // {
    //     
    // }

    public async Task<JsonModel> GetMessage()
    // public void GetMessage()
    {
        var factory = new ConnectionFactory {HostName = _appSettings.RabbitConn};
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: _appSettings.RabbitQueueNameConsume,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);


        // create a consumer that listens on the channel (queue)
        var consumer = new EventingBasicConsumer(channel);
        JsonModel deserializeMessageObject = new JsonModel();


        consumer.Received += (model, eventArgs) =>
        {
            // read the message bytes
            var body = eventArgs.Body.ToArray();

            // convert back to the original string
            var message = Encoding.UTF8.GetString(body);
            Log.Information("Received message: {0}", message);


            //Deserialize JSON Message to Object
            deserializeMessageObject = JsonConvert.DeserializeObject<JsonModel>(message);
            if (deserializeMessageObject != null)
            {
                go = false;
            }
            // channel.BasicAck(eventArgs.DeliveryTag, false);
            


            // Task.Yield();
        };

        channel.BasicConsume(queue: _appSettings.RabbitQueueNameConsume, autoAck: true, consumer: consumer);

        while (go)
        {
            channel.BasicConsume(queue: _appSettings.RabbitQueueNameConsume, autoAck: true, consumer: consumer);

        }

        return deserializeMessageObject;
    }
}