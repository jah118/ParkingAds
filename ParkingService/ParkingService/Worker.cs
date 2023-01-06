using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ParkingService.RabbitMQs;
using ParkingService.util;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using StackExchange.Redis;

namespace ParkingService;

public sealed class Worker : BackgroundService
{
    private readonly IMessageProducer _messagePublisher;


    private readonly AppSettings _appSettings;
    private readonly ConnectionFactory _rabbitFactory;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private readonly IDatabase _redisDatabase;
    private readonly IConnectionMultiplexer _redis;

    public Worker(IMessageProducer messagePublisher, IOptionsMonitor<AppSettings> optionsMonitor,
        IConnectionMultiplexer redis)
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

            // As this is a stub as mentions in Program.cs work will be skipped 
            Task.Run(() =>
            {
                Log.Information("Getting msg ready");

                var redisData = _redisDatabase.StringGet(_appSettings.RedisKey);
                //TODO 
                var trafficData = "THERE IS NO TRAFFIC (PlaceHolder Text) ";

                var parkingMsg = $"{message} teset {redisData} {trafficData}";
                _messagePublisher.SendMessage(parkingMsg);
                Log.Information("Has send message: {string}", parkingMsg);
            });
        };

        _channel.BasicConsume(queue: _appSettings.RabbitQueueNameConsume, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}