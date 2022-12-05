namespace RedisConsumerPullAds.Facade;

public interface IAppSettings
{
    string? RedisConn { get; set; }
    string? RabbitConn { get; set; }
    string? RabbitChannelRedisAds { get; set; }
}