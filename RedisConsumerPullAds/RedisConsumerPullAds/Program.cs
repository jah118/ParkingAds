using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisConsumerPullAds.util;
using Serilog;
using StackExchange.Redis;
// using ConsumerWorker;

namespace RedisConsumerPullAds
{
    // public class ConsumerService : BackgroundService
    // {
    //     private readonly IConnection _rabbitMqConnection;
    //     private readonly IDatabase _redisDatabase;
    //     
    //     private readonly CancellationTokenSource _stoppingCts =
    //         new CancellationTokenSource();
    //     private readonly string _queueName;
    //     private readonly string _redisKey;
    //
    //     public ConsumerService(IConnection rabbitMqConnection, IDatabase redisDatabase, string queueName, string redisKey)
    //     {
    //         _rabbitMqConnection = rabbitMqConnection;
    //         _redisDatabase = redisDatabase;
    //         _queueName = queueName;
    //         _redisKey = redisKey;
    //     }
    //
    //     protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //     {
    //         while (!stoppingToken.IsCancellationRequested)
    //         {
    //             var channel = _rabbitMqConnection.CreateModel();
    //             channel.QueueDeclare(queue: _queueName,
    //                 durable: false,
    //                 exclusive: false,
    //                 autoDelete: false,
    //                 arguments: null);
    //
    //             Log.Information(" [*] Waiting for messages.");
    //             channel.BasicQos(0, 1, false);
    //
    //
    //             var consumer = new EventingBasicConsumer(channel);
    //             // string message = null!;
    //
    //             consumer.Received += (model, eventArgs) =>
    //             {
    //                 var body = eventArgs.Body.ToArray();
    //                 var message = Encoding.UTF8.GetString(body);
    //                 Log.Information("Received message: {0}", message);
    //
    //                 // Save the message in Redis
    //                 _redisDatabase.StringSet(_redisKey, message);
    //                 channel.BasicAck(eventArgs.DeliveryTag, false);
    //             };
    //             channel.BasicConsume(queue: _queueName,
    //                 autoAck: true,
    //                 consumer: consumer);
    //
    //         }
    //         return Task.CompletedTask;
    //     }
    //
    //     public Task StartAsync(CancellationToken cancellationToken)
    //     {
    //         // Set up a consumer to handle incoming messages
    //         var channel = _rabbitMqConnection.CreateModel();
    //         channel.QueueDeclare(queue: _queueName,
    //             durable: false,
    //             exclusive: false,
    //             autoDelete: false,
    //             arguments: null);
    //
    //         Log.Information(" [*] Waiting for messages.");
    //         channel.BasicQos(0, 1, false);
    //
    //
    //         var consumer = new EventingBasicConsumer(channel);
    //         // string message = null!;
    //
    //         consumer.Received += (model, eventArgs) =>
    //         {
    //             var body = eventArgs.Body.ToArray();
    //             var message = Encoding.UTF8.GetString(body);
    //             Log.Information("Received message: {0}", message);
    //
    //             // Save the message in Redis
    //             _redisDatabase.StringSet(_redisKey, message);
    //             channel.BasicAck(eventArgs.DeliveryTag, false);
    //         };
    //         channel.BasicConsume(queue: _queueName,
    //             autoAck: true,
    //             consumer: consumer);
    //
    //         return Task.CompletedTask;
    //     }
    //
    //     public async  Task StopAsync(CancellationToken cancellationToken)
    //     {
    //         _rabbitMqConnection.Close();
    //         _stoppingCts.Cancel();
    //         // return Task.CompletedTask;
    //         await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    //
    //     }
    // }

    static class Program
    {
        public static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Logger.Information("Application Starting");
            // Set up configuration
            // var config = new ConfigurationBuilder()
            //     .AddJsonFile("appsettings.json", optional: false)
            //     .Build();

            // Set up dependency injection
            // var services = new ServiceCollection();
            // IHost host = Host.CreateDefaultBuilder(args)
            //     .ConfigureServices(services =>
            //     {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    AppSettings? options = configuration.GetSection("ApiSettings").Get<AppSettings>();
                    services.AddSingleton(options);
                    // services.AddSingleton<IConnection>(x =>
                    // {
                    //     // Connect to the RabbitMQ server
                    //     var factory = new ConnectionFactory()
                    //     {
                    //         HostName = config["RabbitMQ:Host"],
                    //         UserName = config["RabbitMQ:UserName"],
                    //         Password = config["RabbitMQ:Password"]
                    //     };
                    //     return factory.CreateConnection();
                    // });
                    // services.AddSingleton<IDatabase>(sp =>
                    // {
                    //     // Connect to Redis
                    //     return ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]).GetDatabase();
                    // });
                    // services.AddSingleton(x => config["Queue:Name"]);
                    // services.AddSingleton(x => config["RedisKey:Name"]);
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddHostedService<ConsumerWorker>();

                    // services.AddHostedService<Worker>();
                })
                .Build();

            Log.Information("Application build");
            // var provider = services.BuildServiceProvider();

            // // Run the microservice
            host.Run();

            // var host = provider.GetRequiredService<IHost>();
            // host.Run();
        }
    }
}