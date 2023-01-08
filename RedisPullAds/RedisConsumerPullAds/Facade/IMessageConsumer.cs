using RedisConsumerPullAds.util;

namespace RedisConsumerPullAds.Facade;

public interface IMessageConsumer
{
    string ReceiveMessage(AppSettings appSettings);
}