namespace PullParkingService.util;

public class AppSettings : IAppSettings
{
    public string? URL { get; set; }
    public string? RedisConn { get; set; }
    public string? RabbitConn { get; set; }
    public string? RabbitQueueNameProduce { get; set; }

    public int TimerInterval1 { get; set; } = 60000;
    public int TimerInterval2 { get; set; } = 120000;
}

public interface IAppSettings
{
    public string? URL { get; set; }

    public string? RedisConn { get; set; }
    public string? RabbitConn { get; set; }
    public string? RabbitQueueNameProduce { get; set; }

    public int TimerInterval1 { get; set; }
    public int TimerInterval2 { get; set; }
}