namespace RedisConsumerPullAds.Facade;

public interface IAppSettings
{
    string? RedisConn { get; set; }
    string? RabbitConn { get; set; }
    string? RabbitChannelRedisAds { get; set; }
    
    public string? Host { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    
    public string? QueueName { get; set; }
    public string? RedisConnectionString { get; set; }
    public string? RedisKey { get; set; }

}