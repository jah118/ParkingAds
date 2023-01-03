using RedisConsumerPullAds.util;

namespace RedisConsumerPullAds.Facade;

public interface IRedisWorkerService
{
    Task DataHandling(string msg);
    Task DataHandlingOld(AppSettings appSettings);
}