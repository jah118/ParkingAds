using ParkingAdsAPI.Data;

namespace ParkingAdsAPI.RabbitMQs;

public interface IMessageConsume
{
    public Task<JsonModel> GetMessage();
    // public void GetMessage();
}