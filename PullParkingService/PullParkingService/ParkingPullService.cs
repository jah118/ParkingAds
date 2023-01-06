using Newtonsoft.Json;
using PullParkingService.model;
using Serilog;

namespace PullParkingService;

public interface IParkingPullService
{
    Task<IParkingDataItem> GetData(string baseUrl);
}

public class ParkingPullService : IParkingPullService
{
    public async Task<IParkingDataItem> GetData(string baseUrl)
    {
        // This could be null and is being used with that in mind 
        var item = new ParkingDataItem();

        try
        {
            // Defines HttpClient with IDisposable.
            using var client = new HttpClient();
            //TODO: handle error with http code 200 with msg: "Something bad happened, Sorry.", this is an error that is no catched. 
            // Initiate the Get Request
            using var res = await client.GetAsync(baseUrl);
            if (!res.IsSuccessStatusCode)
            {
                item = new ParkingDataItem
                {
                    Success = false
                };
                Log.Error("unexpected Service response, Received {Data}", res);

                return item;
            }

            //Then get the content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
            using var content = res.Content;
            //Now assign your content to your data variable, by converting into a string using the await keyword.
            var dataString = await content.ReadAsStringAsync();

            //If the data isn't null return log convert the data using newtonsoft JObject Parse class method on the data.
            if (!string.IsNullOrEmpty(dataString))
            {
                //logs data in console,
                // TODO check if this needs parsing add output from request to debug log.
                // TODO Look up serilog structured log and transform data 
                Log.Information("Received {$Data}", dataString);

                List<PInerData> deserializeObject = JsonConvert.DeserializeObject<List<PInerData>>(dataString);
                
                
                item = new ParkingDataItem
                {
                    Success = true,
                    Content = deserializeObject
                };
                return item;
            }

            return item;
        }
        catch (Exception exception)
        {
            Log.Error("Exception Hit in ADPullService------------\n {$Data}", exception);
            throw;
        }
    }
}