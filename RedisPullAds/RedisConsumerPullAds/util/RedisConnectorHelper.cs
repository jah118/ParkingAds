using StackExchange.Redis;

namespace RedisConsumerPullAds.util;

public class RedisConnectorHelper
{
    static RedisConnectorHelper()
    {
        RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("localhost:6379,password=eYVX7EwVmmxKPCDmwMtyKVge8oLd2t81");
        });
    }

    private static Lazy<ConnectionMultiplexer> lazyConnection;

    public static ConnectionMultiplexer Connection
    {
        get { return lazyConnection.Value; }
    }
}