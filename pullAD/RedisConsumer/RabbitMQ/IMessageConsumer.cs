namespace RedisConsumer.RabbitMQ;

public interface IMessageConsumer
{
    string ReceiveMessage();
}