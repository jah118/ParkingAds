﻿// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using RedisConsumerPullAds.Facade;
using RedisConsumerPullAds.util;
using Serilog;
using StackExchange.Redis;

namespace RedisConsumerPullAds;

public static class Program
{
    private static IAppSettings? _appSettings;
    private static IMessageConsumer _messageConsumer;
    private static IDatabase _redis;

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        //setups
        var builder = new ConfigurationBuilder();
        IAppSettingsHandler settingsHandler = new AppSettingsHandler(builder);
        _appSettings = settingsHandler.AppSettings;

        // var serviceProvider = new ServiceCollection()
        //     .AddLogging()
        //     .AddSingleton<IMessageConsumer, RabbitMQConsumer>()
        //     .AddSingleton<IRedisWorkerService, RedisWorkerService>()
        //     .BuildServiceProvider();


        // RedisConnectionFactory.SetSettings(_appSettings);
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
            Console.WriteLine("Hello, World!");

            //do the actual work here
            //var worker = serviceProvider.GetService<IRedisWorkerService>();
            //worker?.DataHandling();
            Console.WriteLine("tesa");

            // DataHandling();
            //TODO Do something better like wtf is this.....
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S))
            {
                // do something
            }
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