using pullADs.Facade;
using Serilog;

namespace pullADs
{
    public class AdPullService : IAdPullService
    {
        public async Task<IAdItem> GetAd(string baseUrl)
        {
            // This could be null and is being used with that in mind 
            var adItem = new AdItem();

            try
            {
                // Defines HttpClient with IDisposable.
                using HttpClient client = new HttpClient();
                //TODO: handle error with http code 200 with msg: "Something bad happened, Sorry.", this is an error that is no catched. 
                // Initiate the Get Request
                using HttpResponseMessage res = await client.GetAsync(baseUrl);
                if (!res.IsSuccessStatusCode)
                {
                    adItem = new AdItem
                    {
                        Success = false
                    };
                    Log.Error("unexpected Service response, Received {Data}", res);

                    return adItem;
                }

                //Then get the content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
                using HttpContent content = res.Content;
                //Now assign your content to your data variable, by converting into a string using the await keyword.
                string dataString = await content.ReadAsStringAsync();

                //If the data isn't null return log convert the data using newtonsoft JObject Parse class method on the data.
                if (!string.IsNullOrEmpty(dataString))
                {
                    //logs data in console,
                    // TODO check if this needs parsing add output from request to debug log.
                    // TODO Look up serilog structured log and transform data 
                    Log.Information("Received {$Data}", dataString);
                    Log.Information("Received {Data}", dataString);

                    adItem = new AdItem(dataString)
                    {
                        Success = true
                    };
                    return adItem;
                }

                return adItem;
            }
            catch (Exception exception)
            {
                Log.Error("Exception Hit------------ {$Data}", exception);
                throw;
            }
        }
    }
}