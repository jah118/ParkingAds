namespace PullADsToMQ.RabbitMQ;

public interface IMessageProducer
{
    void SendMessage<T>(T message);
}