using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Splitter.Data;
using Splitter.RabbitMQs;
using Splitter.util;

namespace Splitter;

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

            // the splitter works by, reading a list from config an converting it to queueNames where other service can pick up the msg an do work
            Task.Run(() =>
            {
                Log.Information("Getting msg ready");

                if (_appSettings.RabbitQueueNameProduceList != null)
                {
                    var count = _appSettings.RabbitQueueNameProduceList.Count();
                    JsonModel deserializeObject = JsonConvert.DeserializeObject<JsonModel>(message);
                    deserializeObject.Sesssion.SplitCounter =
                        count; //TODO do this the smart way, need monitor information and add logic to decide which aggregator to chose
                    
                    for (var a = 0; a <= count - 1; a += 1)
                    {
                        var queueName =
                            $"{_appSettings.RabbitQueueNameConsume}.{_appSettings.RabbitQueueNameProduceList[a]}";
                        deserializeObject.TopicKey = queueName;

                        //TODO: update json message with count size so splits i known for the aggregator
                        //TODO: DO actual splitter work on msg, do not send whole msg 
                        _messagePublisher.SendMessage(deserializeObject, queueName);
                        Log.Information("Has send message: {string} onto {string}", deserializeObject, queueName);
                    }
                }
            });
        };

        _channel.BasicConsume(queue: _appSettings.RabbitQueueNameConsume, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}