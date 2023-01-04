using StackExchange.Redis;

namespace RedisConsumerPullAds.util;

public class RedisConnectionFactory2
{
    private static Lazy<ConnectionMultiplexer> lazyConnection;

    static RedisConnectionFactory2()
    {
        lazyConnection = new Lazy<ConnectionMultiplexer>(() => { return ConnectionMultiplexer.Connect("localhost"); });
    }

    public static ConnectionMultiplexer Connection
    {
        get { return lazyConnection.Value; }
    }
}