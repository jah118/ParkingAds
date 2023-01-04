using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RouteSlip.RabbitMQs;
using RouteSlip.util;
using Serilog;

namespace RouteSlip;

public sealed class ConsumerWorker : BackgroundService
{
    private readonly IMessageProducer _messagePublisher;


    private AppSettings _appSettings;
    private ConnectionFactory _factory;
    private IConnection _connection;
    private IModel _channel;

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
            // {index}|SuperHero{10000+index}|Fly,Eat,Sleep,Manga|1|{DateTime.UtcNow.ToLongDateString()}|0|0
            // is received here
            var message = Encoding.UTF8.GetString(body);
            Log.Information("Received message: {0}", message);
            Console.WriteLine(" [x] Received {0}", message);


            Task.Run(() =>
            {
                Log.Information("Getting msg ready");

                //TODO DO WORK TO  message
                _messagePublisher.SendMessage(message + "teset");
                Log.Information("Has send message: {0}", message);

                // // BackgroundService is a Singleton service
                // // IHeroesRepository is declared a Scoped service
                // // by definition a Scoped service can't be consumed inside a Singleton
                // // to solve this, we create a custom scope inside the Singleton and 
                // // perform the insertion.
                // using (var scope = _sp.CreateScope())
                // {
                //     var db = scope.ServiceProvider.GetRequiredService<IHeroesRepository>();
                //     db.Create(hero);
                // }


                // Save the message in Redis
                //_redisDatabase.StringSet(options.RedisKey, message);
                //_channel.BasicAck(eventArgs.DeliveryTag, false);
            });
        };

        _channel.BasicConsume(queue: _appSettings.RabbitQueueNameConsume, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}