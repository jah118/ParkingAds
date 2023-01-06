using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RouteSlip.Data;
using RouteSlip.RabbitMQs;
using RouteSlip.util;
using Serilog;

namespace RouteSlip;

public sealed class ConsumerWorker : BackgroundService
{
    private readonly IMessageProducer _messagePublisher;


    private readonly AppSettings _appSettings;
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public ConsumerWorker(IMessageProducer messagePublisher, IOptionsMonitor<AppSettings> optionsMonitor)
    {
        _appSettings = optionsMonitor.CurrentValue;
        _messagePublisher = messagePublisher;

        _factory = new ConnectionFactory()
        {
            HostName = _appSettings.RabbitConn,
            UserName = "guest",
            Password = "guest"
        };


        // _factory = new ConnectionFactory();
        // _factory.Uri = new Uri("amqp://guest:guest@localhost:5672/");


        _connection = _factory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _appSettings.RabbitQueueNameConsume,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // when the service is stopping
        // dispose these references
        // to prevent leaks
        if (stoppingToken.IsCancellationRequested)
        {
            _channel.Dispose();
            _connection.Dispose();
            return Task.CompletedTask;
        }

        // create a consumer that listens on the channel (queue)
        var consumer = new EventingBasicConsumer(_channel);

        // handle the Received event on the consumer
        // this is triggered whenever a new message
        // is added to the queue by the producer
        consumer.Received += (model, eventArgs) =>
        {
            // read the message bytes
            var body = eventArgs.Body.ToArray();

            // convert back to the original string
            var message = Encoding.UTF8.GetString(body);
            Log.Information("Received message: {0}", message);
            Console.WriteLine(" [x] Received {0}", message);


            Task.Run(() =>
            {
                Log.Information("Getting msg ready");
                //TODO DO WORK TO  message
                JsonModel deserializeObject = JsonConvert.DeserializeObject<JsonModel>(message);
                deserializeObject.TopicKey = _appSettings.RabbitQueueNameProduce;
                deserializeObject.Sesssion.AggregatorTarget =
                    "aggregator_A"; //TODO do this the smart way, need monitor information and add logic to decide which aggregator to chose
                
                
                _messagePublisher.SendMessage(deserializeObject);
                var serializeObject = JsonConvert.SerializeObject(deserializeObject);

                Log.Information("Has send message: {0} with content \n{JSON}", deserializeObject, serializeObject);
            });
        };

        _channel.BasicConsume(queue: _appSettings.RabbitQueueNameConsume, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}