namespace pullADs.Facade;

public interface IAppSettings
{
    string? AdUrl { get; set; }

    string? RabbitConn { get; set; }
    string? RabbitChannelAds { get; set; }

    int TimerInterval1 { get; }
    int TimerInterval2 { get; }
}