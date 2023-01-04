using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RedisConsumerPullAds.Facade;
using RedisConsumerPullAds.util;
using Serilog;
using StackExchange.Redis;

namespace RedisConsumerPullAds;

public sealed class ConsumerWorker : BackgroundService
{
    //private readonly IConfiguration _configuration;


    private IAppSettings options;
    private ConnectionFactory _connectionFactory;
    private IConnection _connection;

    private IModel _channel;
    // got error
    // private readonly IConnection _rabbitMqConnection;
    // private readonly IDatabase _redisDatabase;
    //
    // private readonly CancellationTokenSource _stoppingCts =
    //     new CancellationTokenSource();
    //
    // private readonly string _queueName;
    // private readonly string _redisKey;

    //got error
    // public ConsumerWorker(IConnection rabbitMqConnection, IDatabase redisDatabase, string queueName, string redisKey)
    // {
    //     _rabbitMqConnection = rabbitMqConnection;
    //     _redisDatabase = redisDatabase;
    //     _queueName = queueName;
    //     _redisKey = redisKey;
    // }

    public ConsumerWorker(IAppSettings options)
    {
        this.options = options;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory {HostName = options.RabbitConn};
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _channel.BasicQos(0, 1, false);
        Log.Information(" [*] Waiting for messages.");

        return base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _connection.Close();
        Log.Information("RabbitMQ connection is closed.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Starting");
        // while (!stoppingToken.IsCancellationRequested)
        // {

        // var factory = new ConnectionFactory {HostName = options.RabbitConn};
        // using var connection = factory.CreateConnection();
        // using var channel = connection.CreateModel();
        // while (!stoppingToken.IsCancellationRequested)
        // {

        //var channel = _rabbitMqConnection.CreateModel();


        // new  https://codeburst.io/get-started-with-rabbitmq-2-consume-messages-using-hosted-service-e7e6a20b15a6 ############

        stoppingToken.ThrowIfCancellationRequested();

        // _channel.QueueDeclare(queue: options.QueueName,
        //     durable: true,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: null);
        //
        // _channel.BasicQos(0, 1, false);
        // Log.Information(" [*] Waiting for messages.");

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received +=  (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Log.Information("Received message: {0}", message);
            Console.WriteLine("asd");
            Console.WriteLine(message);
            // Save the message in Redis
            //_redisDatabase.StringSet(options.RedisKey, message);
            _channel.BasicAck(eventArgs.DeliveryTag, false);
            Task.Yield();
            
            
            // try
            // {
            //     // Save the message in Redis
            //     //_redisDatabase.StringSet(options.RedisKey, message);
            //     _channel.BasicAck(eventArgs.DeliveryTag, false);
            //     await Task.Yield();
            // }
            // // catch (REDIS EROOR)
            // // {
            // // THIS IS TODO
            // //     Log.Error("RabbitMQ is closed!");
            // // }
            // catch (AlreadyClosedException)
            // {
            //     Log.Error("RabbitMQ is closed!");
            //     await Task.Yield();
            // }
            // catch (Exception e)
            // {
            //     Log.Error("ERROR: \n {e}", e);
            // }

            // await Task.CompletedTask;
        };

        Log.Information("Worker running at: {time}", DateTimeOffset.Now);
        // await Task.Delay(1000, stoppingToken);

        _channel.BasicConsume(queue: options.RabbitChannelRedisAds,
            autoAck: true,
            consumer: consumer);
        // }
        await Task.CompletedTask;

        // return Task.CompletedTask;
        
    }
    //
    // public Task StartAsync(CancellationToken cancellationToken)
    // {
    //     // Set up a consumer to handle incoming messages
    //     var channel = _rabbitMqConnection.CreateModel();
    //     channel.QueueDeclare(queue: _queueName,
    //         durable: false,
    //         exclusive: false,
    //         autoDelete: false,
    //         arguments: null);
    //
    //     Log.Information(" [*] Waiting for messages.");
    //     channel.BasicQos(0, 1, false);
    //
    //
    //     var consumer = new EventingBasicConsumer(channel);
    //     // string message = null!;
    //
    //     consumer.Received += (model, eventArgs) =>
    //     {
    //         var body = eventArgs.Body.ToArray();
    //         var message = Encoding.UTF8.GetString(body);
    //         Log.Information("Received message: {0}", message);
    //
    //         // Save the message in Redis
    //         _redisDatabase.StringSet(_redisKey, message);
    //         channel.BasicAck(eventArgs.DeliveryTag, false);
    //     };
    //     channel.BasicConsume(queue: _queueName,
    //         autoAck: true,
    //         consumer: consumer);
    //
    //     return Task.CompletedTask;
    // }
    //
    // public async Task StopAsync(CancellationToken cancellationToken)
    // {
    //     _rabbitMqConnection.Close();
    //     _stoppingCts.Cancel();
    //     // return Task.CompletedTask;
    //     await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    // }
}