using PullADsToMQ.util;

namespace PullADsToMQ.Facade;

public interface IAppSettingsHandler
{
    AppSettings AppSettings { get; set; }
}