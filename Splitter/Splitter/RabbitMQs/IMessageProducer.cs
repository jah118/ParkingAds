namespace Splitter.RabbitMQs
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message, string QueueName);
    }
}