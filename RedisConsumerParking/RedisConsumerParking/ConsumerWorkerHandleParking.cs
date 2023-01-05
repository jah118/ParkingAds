using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisConsumerParking.util;
using Serilog;
using StackExchange.Redis;

namespace RedisConsumerParking;

public sealed class ConsumerWorkerHandleParking : BackgroundService
{
    private readonly AppSettings _appSettings;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ConnectionFactory _rabbitFactory;
    private readonly IDatabase _redisDatabase;

    public ConsumerWorkerHandleParking(IOptionsMonitor<AppSettings> optionsMonitor)
    {
        _appSettings = optionsMonitor.CurrentValue;

        _redisDatabase = RedisConnectorHelper.Connection.GetDatabase();

        _rabbitFactory = new ConnectionFactory {HostName = _appSettings.RabbitConn};

        _connection = _rabbitFactory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            _appSettings.QueueName1,
            true,
            false,
            false,
            null);
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

            var message = Encoding.UTF8.GetString(body);
            Log.Information("Received message: {0}", message);
            Console.WriteLine(" [x] Received {0}", message);


            Task.Run(() =>
            {
                // Save the message in Redis
                _redisDatabase.StringSet(_appSettings.RedisKey1, message);
                Log.Information("StringSet to redis with: {}", message);
                //_channel.BasicAck(eventArgs.DeliveryTag, false);
            });
        };

        _channel.BasicConsume(_appSettings.QueueName1, true, consumer);

        return Task.CompletedTask;
    }
}