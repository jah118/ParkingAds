﻿namespace PullADsToMQ.Facade;

public interface IAdItem
{
    bool Success { get; set; }
    string Content { get; set; }
}