namespace pullADs.util;

public interface IApiSettings
{
    string? AdUrl { get; set; }
    string? RedisConn { get; set; }
    int TimerInterval1 { get; }
    int TimerInterval2 { get; }
}