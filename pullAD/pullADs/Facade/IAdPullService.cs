namespace pullADs;

public interface IAdPullService
{
    Task<AdItem> GetAd(string baseUrl);
}