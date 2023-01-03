// See https://aka.ms/new-console-template for more information

using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisConsumerPullAds.Facade;
using RedisConsumerPullAds.RabbitMQ;
using RedisConsumerPullAds.util;
using Serilog;
using StackExchange.Redis;

namespace RedisConsumerPullAds;

public static class Program
{
    private static IAppSettings? Settings { get; set; }
    private static IMessageConsumer _messageConsumer;
    private static IDatabase _redis;

    private static Task  Main(string[] args)
    {
        Console.WriteLine("Hello World!");


        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        
        var configuration = configurationBuilder.Build();

        //setups
        var builder = new ConfigurationBuilder();
        IAppSettingsHandler settingsHandler = new AppSettingsHandler(builder);
        Settings = settingsHandler.AppSettings;
        Console.WriteLine("Hello World!");
        var serviceProvider = new ServiceCollection()
            .Configure<AppSettings>(configuration.GetSection("ApiSettings"))
            .AddSingleton<IRedisWorkerService, RedisWorkerService>()
            .BuildServiceProvider();
        

        // RedisConnectionFactory.SetSettings(Settings);
        // _redis = RedisConnectionFactory.Database;


        using var log = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Build())
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = log;
        Log.Logger.Information("Application Starting");
        Log.Information("The global logger has been configured");

        
        
        
        try
        {
            var redisWorker = new RedisWorkerService(Settings);

            Console.WriteLine("Hello,123123 World!");

            // var appSettings = serviceProvider.GetService<IOptions<AppSettings>>().Value;

            // Console.WriteLine(appSettings.ToString());
            Console.WriteLine(Settings);

            // var worker = serviceProvider.GetService<RedisWorkerService>();
            // var Listener = serviceProvider.GetService<Consumer>();
            //
            // worker?.DataHandling(appSettings);


            var factory = new ConnectionFactory {HostName = Settings.RabbitConn};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(Settings.RabbitChannelRedisAds,
                true,
                false,
                false,
                null);

            channel.BasicQos(0, 1, false);
            Log.Information(" [*] Waiting for messages.");


            var consumer = new EventingBasicConsumer(channel);
            string message = null!;
            consumer.Received += (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                message = Encoding.UTF8.GetString(body);
                Log.Information(" [x] Received {message}", message);
                redisWorker.DataHandling(message);
                
                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 5000);
                
                
                // Note: it is possible to access the channel via
                //       ((EventingBasicConsumer)sender).Model here
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            channel.BasicConsume(Settings.RabbitChannelRedisAds,
                false,
                consumer);


            Console.WriteLine("tesa");

            // DataHandling();
            // //TODO Do something better like wtf is this.....
            // while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S))
            // {
            //     // do something
            // }

            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // /// <summary>
    // ///     Gets Ad data, format it and send it to msg queue
    // /// </summary>
    // private static async Task DataHandling()
    // {
    //     try
    //     {
    //         Log.Information("Started DataHandling");
    //
    //         var data = _messageConsumer.ReceiveMessage();
    //
    //         Log.Debug("AD data received over bus: {@Data}", data);
    //
    //         if (string.IsNullOrEmpty(data))
    //             //TODO  add alert bad data on queue - monitoring
    //             Log.Error("ERROR, No data received");
    //         else
    //             //TODO add to redis 
    //             Log.Information("Adding this redis: {@Data}", data);
    //     }
    //     catch (Exception e)
    //     {
    //         //TODO: do some smart error handling  or delete this try catch and let service fail
    //         Console.WriteLine(e);
    //         Log.Error("Exception Hit------------ ");
    //     }
    // }
    //https://github.com/StackExchange/StackExchange.Redis/issues/1542

    // public static void ReadData()
    // {
    //     var cache = RedisConnectionFactory.Connection.GetDatabase();
    //     var devicesCount = 10000;
    //     for (int i = 0; i < devicesCount; i++)
    //     {
    //         var value = cache.StringGet($"Device_Status:{i}");
    //         Console.WriteLine($"Valor={value}");
    //     }
    // }
    //
    //
    // public static void SaveData(IDatabase _redis, string data)
    // {
    // }
    //
    // public static void SaveBigData()
    // {
    //     var devicesCount = 10000;
    //     var rnd = new Random();
    //     var cache = RedisConnectionFactory.Connection.GetDatabase();
    //
    //     for (int i = 1; i < devicesCount; i++)
    //     {
    //         var value = rnd.Next(0, 10000);
    //         cache.StringSet($"Device_Status:{i}", value);
    //     }
    // }
}