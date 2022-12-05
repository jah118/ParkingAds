namespace RedisConsumerPullAds.RabbitMQ;

public interface IMessageConsumer
{
    string ReceiveMessage();
}