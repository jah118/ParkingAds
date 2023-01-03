using RedisConsumerPullAds.Facade;
using StackExchange.Redis;

namespace RedisConsumerPullAds.util;

public class RedisConnectionFactory : IRedisConnectionFactory
{
    private static Lazy<ConnectionMultiplexer> _connection;

    private static readonly Lazy<ConnectionMultiplexer> LazyConnection;
    private static IAppSettings? _appSettings;

    /// <summary>
    ///     Singleton
    /// </summary>
    // static RedisConnectionFactory()  
    // {  
    //     lazyConnection = new Lazy<ConnectionMultiplexer>(() =>  
    //     {  
    //         return ConnectionMultiplexer.Connect("localhost");  
    //     });  
    // }  
    //   
    // private static Lazy<ConnectionMultiplexer> lazyConnection;          
    //
    // public static ConnectionMultiplexer Connection  
    // {  
    //     get  
    //     {  
    //         return lazyConnection.Value;  
    //     }  
    // }  
    // private RedisConnectionFactory()
    // {
    // }

    // public RedisConnectionFactory()
    // {
    //     LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    //         ConnectionMultiplexer.Connect(_appSettings?.RedisConn ?? throw new InvalidOperationException()));
    // }
    public RedisConnectionFactory(IAppSettings appSettings)
    {
        _appSettings = appSettings;
        _connection  = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(_appSettings?.RedisConn ?? throw new InvalidOperationException()));
    }
    
    public ConnectionMultiplexer GetConnection()
    {
        return _connection.Value;
    }

    // public static ConnectionMultiplexer Connection => LazyConnection.Value;

    public IDatabase Database => _connection.Value.GetDatabase();

    // public static void SetSettings(IAppSettings? appSettings)
    // {
    //     _appSettings = appSettings;
    // }
}

public interface IRedisConnectionFactory
{
    
}