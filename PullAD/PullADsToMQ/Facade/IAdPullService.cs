﻿namespace PullADsToMQ.Facade;

public interface IAdPullService
{
    Task<IAdItem> GetAd(string baseUrl);
}