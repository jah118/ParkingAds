﻿namespace ParkingService.RabbitMQs
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message);
    }
}
