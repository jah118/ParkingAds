namespace RedisConsumer.Facade;

public interface IMessageConsumer
{
    
    void SendMessage<T>(T message);
    string ReceiveMessage();
}