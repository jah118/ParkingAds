using RedisConsumerPullAds.Facade;

namespace RedisConsumerPullAds.util;

public class AppSettings : IAppSettings
{
    public string? RedisConn { get; set; }
    public string? RabbitConn { get; set; }
    public string? RabbitChannelRedisAds { get; set; }
    
    public string? Host { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    
    public string? QueueName1 { get; set; }
    public string? QueueName2 { get; set; }
    public string? RedisConnectionString { get; set; }
    public string? RedisKey1 { get; set; }
    public string? RedisKey2 { get; set; }

}