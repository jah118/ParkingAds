namespace pullADs
{
    public class AdPullService : IAdPullService
    {
        public async Task<AdItem> GetAd(string baseUrl)
        {
            // This could be null and is being used with that in mind 
            AdItem adItem = new AdItem();

            try
            {
                // Defines HttpClient with IDisposable.
                using HttpClient client = new HttpClient();
                // Initiate the Get Request
                using HttpResponseMessage res = await client.GetAsync(baseUrl);
                if (res.IsSuccessStatusCode)
                {
                    //Then get the content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
                    using HttpContent content = res.Content;
                    //Now assign your content to your data variable, by converting into a string using the await keyword.
                    string dataString = await content.ReadAsStringAsync();

                    //If the data isn't null return log convert the data using newtonsoft JObject Parse class method on the data.
                    if (!string.IsNullOrEmpty(dataString))
                    {
                        //logs data in console,
                        //TODO check if this needs parsing add output from request to debug log.
                        Console.WriteLine("data------------{0}", dataString);
                        adItem = new AdItem(dataString);
                        return adItem;
                    }

                    Console.WriteLine("NO Data----------");
                    return adItem; //TODO do better than null
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception Hit------------"); //changes this over to seq data logging
                Console.WriteLine(exception);
            }

            return adItem;
        }
    }
}