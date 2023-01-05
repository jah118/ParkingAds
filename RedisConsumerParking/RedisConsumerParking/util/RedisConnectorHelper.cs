using StackExchange.Redis;

namespace RedisConsumerParking.util;

public static class RedisConnectorHelper
{
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection;

    static RedisConnectorHelper()
    {
        RedisConnectorHelper.LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect("localhost:6379,password=eYVX7EwVmmxKPCDmwMtyKVge8oLd2t81"));
    }

    public static ConnectionMultiplexer Connection => LazyConnection.Value;
}