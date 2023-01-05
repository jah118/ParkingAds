using RedisConsumerParking.util;

namespace RedisConsumerParking.Facade;

public interface IMessageConsumer
{
    string ReceiveMessage(AppSettings appSettings);
}