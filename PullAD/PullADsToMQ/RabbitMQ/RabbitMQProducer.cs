﻿using System.Text;
using Newtonsoft.Json;
using PullADsToMQ.Facade;
using RabbitMQ.Client;
using Serilog;

namespace PullADsToMQ.RabbitMQ;

public class RabbitMQProducer : IMessageProducer
{
    private readonly IAppSettings _appSettings;

    public RabbitMQProducer(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public void SendMessage<T>(T message)
    {
        Log.Information("Started producing");
        // TODO USE app settings
        var factory = new ConnectionFactory { HostName = _appSettings.RabbitConn };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: _appSettings.RabbitChannelAds,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(exchange: "",
            routingKey: _appSettings.RabbitChannelAds,
            basicProperties: properties,
            body: body);
        //TODO DO better log msg 
        Log.Information(" [x] Sent {0}", message);
    }
}
