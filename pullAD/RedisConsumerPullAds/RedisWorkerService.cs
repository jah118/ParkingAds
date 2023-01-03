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

    public RedisWorkerService(IAppSettings appSettings, IMessageConsumer consumer)
    {
        _appSettings = appSettings;
        _messageConsumer = consumer;
        setup();
    }


    /// <summary>
    ///     Gets Ad data, format it and send it to msg queue
    /// </summary>
    public Task DataHandling()
    {
        try
        {
            Log.Information("Started DataHandling");

            var data = _messageConsumer.ReceiveMessage();

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

    private void setup()
    {
        if (_appSettings is null) throw new ConfigurationMissingException();

        RedisConnectionFactory.SetSettings(_appSettings);
        _redis = RedisConnectionFactory.Database;
    }

    private void ReadData()
    {
        var cache = RedisConnectionFactory.Connection.GetDatabase();
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
        throw new NotImplementedException("todo");
    }
}