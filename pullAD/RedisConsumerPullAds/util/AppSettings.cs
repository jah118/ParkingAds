using RedisConsumerPullAds.Facade;

namespace RedisConsumerPullAds.util;

public class AppSettings : IAppSettings
{
    public string? RedisConn { get; set; }
    public string? RabbitConn { get; set; }
    public string? RabbitChannelRedisAds { get; set; }
}