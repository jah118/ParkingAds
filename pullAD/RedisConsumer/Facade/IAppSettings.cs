﻿namespace RedisConsumer.Facade;

public interface IAppSettings
{
    string? RedisConn { get; set; }
    string? RabbitConn { get; set; }
}