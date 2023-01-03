using RedisConsumerPullAds.Facade;
using StackExchange.Redis;

namespace RedisConsumerPullAds.util;

public class RedisConnectionFactory
{
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
    static RedisConnectionFactory()
    {
        LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(_appSettings?.RedisConn ?? throw new InvalidOperationException()));
    }


    public static ConnectionMultiplexer Connection => LazyConnection.Value;

    public static IDatabase Database => Connection.GetDatabase();

    public static void SetSettings(IAppSettings? appSettings)
    {
        _appSettings = appSettings;
    }
}