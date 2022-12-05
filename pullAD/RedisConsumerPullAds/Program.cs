// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using RedisConsumerPullAds.Facade;
using RedisConsumerPullAds.RabbitMQ;
using RedisConsumerPullAds.util;
using Serilog;

namespace RedisConsumerPullAds;

public static class Program
{
    private static IAppSettings _appSettings;
    private static IMessageConsumer _messageConsumer;

    private static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        IAppSettingsHandler settingsHandler = new AppSettingsHandler(builder);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Build())
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Console.WriteLine("Hello, World!");
            DataHandling();
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

    /// <summary>
    ///     Gets Ad data, format it and send it to msg queue
    /// </summary>
    private static async Task DataHandling()
    {
        try
        {
            Log.Information("Started DataHandling");

            var data = _messageConsumer.ReceiveMessage();

            Log.Debug("AD data received over bus: {@Data}", data);

            if (string.IsNullOrEmpty(data))
                //TODO  add alert bad data on queue 
                Log.Error("ERROR, No data received");
            else
                //TODO add to redis 
                Log.Information("Adding this redis: {@Data}", data);
        }
        catch (Exception e)
        {
            //TODO: do some smart error handling  or delete this try catch and let service fail
            Console.WriteLine(e);
            Log.Error("Exception Hit------------ ");
        }
    }
}