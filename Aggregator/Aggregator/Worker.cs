using System.Text;
using Aggregator.Data;
using Aggregator.RabbitMQs;
using Aggregator.util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using StackExchange.Redis;

namespace Aggregator;

public sealed class Worker : BackgroundService
{
    private readonly IMessageProducer _messagePublisher;

    private readonly AppSettings _appSettings;
    private readonly ConnectionFactory _rabbitFactory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly QueueDeclareOk _queueName;

    private readonly IDatabase _redisDatabase;
    private readonly IConnectionMultiplexer _redis;

    public Worker(IMessageProducer messagePublisher, IOptionsMonitor<AppSettings> optionsMonitor,IConnectionMultiplexer redis)
    {
        _appSettings = optionsMonitor.CurrentValue;
        
        _redis = redis;
        _redisDatabase = _redis.GetDatabase();


        _messagePublisher = messagePublisher;


        _rabbitFactory = new ConnectionFactory()
        {
            HostName = _appSettings.RabbitConn,
            UserName = "guest",
            Password = "guest"
        };
        // _rabbitFactory = new ConnectionFactory();
        // _rabbitFactory.Uri = new Uri("amqp://guest:guest@localhost:5672/");

        _connection = _rabbitFactory.CreateConnection();

        _channel = _connection.CreateModel();

        _queueName =  _channel.QueueDeclare(
            queue: _appSettings.RabbitQueueNameConsume,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        // _channel.QueueBind(queue: _queueName,
        //     exchange: "logs",
        //     routingKey: "");
        //
        // _channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");

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

            // As this is a stub as mentions in Program.cs work will be skipped 
            Task.Run(() =>
            {
                Log.Information("Getting msg ready Aggregator");
                //TODO do actuel msg building and wait for all msg
                
                //Pull redis data make this in content enricher
                var redisData = _redisDatabase.StringGet(_appSettings.RedisKey);

                //Deserialize JSON Message to Object
                JsonModel deserializeMessageObject = JsonConvert.DeserializeObject<JsonModel>(message);
                //Add info to object
                deserializeMessageObject!.TopicKey = _appSettings.RabbitQueueNameProduce;
                //content enrich
                deserializeMessageObject.AdData = redisData.ToString();
                

                _messagePublisher.SendMessage(deserializeMessageObject);
                var serializeObject = JsonConvert.SerializeObject(deserializeMessageObject);

                Log.Information("Has send message: {JSON}", serializeObject);
            });
        };

        _channel.BasicConsume(queue: _appSettings.RabbitQueueNameConsume, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}