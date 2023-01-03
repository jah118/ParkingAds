using Microsoft.Extensions.Options;
using RedisConsumerPullAds.Facade;
using RedisConsumerPullAds.util;
using Serilog;
using StackExchange.Redis;

namespace RedisConsumerPullAds;

public class RedisWorkerService : IRedisWorkerService
{
    private readonly IAppSettings? _appSettings;
    private readonly IMessageConsumer _messageConsumer;
    private IDatabase _redis;

    public RedisWorkerService(IOptionsMonitor<AppSettings> optionsMonitor, IMessageConsumer consumer)
    {
        // _appSettings = appSettings;
        _messageConsumer = consumer;
        Setup();
    }

    public RedisWorkerService(IAppSettings appSettings)
    {
        _appSettings = appSettings;
        Setup();
    }


    /// <summary>
    ///     Gets Ad data, format it and send it to msg queue
    /// </summary>
    public Task DataHandling(string msg)
    {
        try
        {
            Log.Information("Started DataHandling");


            Log.Debug("AD data received over bus: {@Data}", msg);

            if (!string.IsNullOrEmpty(msg))
            {
                //TODO add to redis 
                Log.Information("Adding this redis: {@Data}", msg);
                SaveData(_redis, msg);
            }

            //TODO  add alert bad data on queue - monitoring
            Log.Error("ERROR, No data received");
        }
        catch (Exception e)
        {
            //TODO: do some smart error handling  or delete this try catch and let service fail
            Console.WriteLine(e);
            Log.Error("Exception Hit------------ ");
        }

        return Task.CompletedTask;
    }

    public Task DataHandlingOld(AppSettings appSettings)
    {
        try
        {
            Log.Information("Started DataHandling");

            var data = _messageConsumer.ReceiveMessage(appSettings);

            Log.Debug("AD data received over bus: {@Data}", data);

            if (!string.IsNullOrEmpty(data))
            {
                //TODO add to redis 
                Log.Information("Adding this redis: {@Data}", data);
                SaveData(_redis, data);
            }

            //TODO  add alert bad data on queue - monitoring
            Log.Error("ERROR, No data received");
        }
        catch (Exception e)
        {
            //TODO: do some smart error handling  or delete this try catch and let service fail
            Console.WriteLine(e);
            Log.Error("Exception Hit------------ ");
        }

        return Task.CompletedTask;
    }


    private void Setup()
    {
        if (_appSettings is null) throw new ConfigurationMissingException();

        RedisConnectionFactory(_appSettings);
        _redis = RedisConnectionFactory.Database;
    }

    private void ReadData()
    {
        var cache = RedisConnectionFactory..GetDatabase();
        var devicesCount = 10000;
        for (var i = 0; i < devicesCount; i++)
        {
            var value = cache.StringGet($"Device_Status:{i}");
            Console.WriteLine($"Valor={value}");
        }
    }


    public void SaveData(IDatabase _redis, string data)
    {
        Console.WriteLine(_redis);
        Console.WriteLine(data);
        // throw new NotImplementedException("todo");
        Console.WriteLine("does nothing");
    }
}