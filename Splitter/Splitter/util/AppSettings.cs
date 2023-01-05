namespace Splitter.util;

public class AppSettings : IAppSettings
{
    public string? RabbitConn { get; set; }
    public string? RabbitQueueNameConsume { get; set; }
    public List<string>? RabbitQueueNameProduceList { get; set; }
}

public interface IAppSettings
{
    public string? RabbitConn { get; set; }
    public string? RabbitQueueNameConsume { get; set; }
    public List<string>? RabbitQueueNameProduceList { get; set; }
}