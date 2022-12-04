namespace pullADs.Facade;

public interface IMessageProducer
{
    void SendMessage<T> (T message);
}