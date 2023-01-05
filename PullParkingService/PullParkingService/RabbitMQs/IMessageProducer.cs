﻿namespace PullParkingService.RabbitMQs
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message);
    }
}
