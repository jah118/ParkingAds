namespace pullADs
{
    public class AdPullService
    {
        //public AdPullService(string content, string url)
        //{
        //    Content = content;
        //}

        public async Task<AdItem> GetAd(string baseUrl)
        {
            try
            {
                // Defines HttpClient with IDisposable.
                using (HttpClient client = new HttpClient())
                {
                    // Initiate the Get Request
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                    {
                        if (res.IsSuccessStatusCode)
                        {
                            //Then get the content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
                            using (HttpContent content = res.Content)
                            {
                                //Now assign your content to your data variable, by converting into a string using the await keyword.
                                var data = await content.ReadAsStringAsync();
                                //If the data isn't null return log convert the data using newtonsoft JObject Parse class method on the data.
                                if (data != null)
                                {
                                    //logs data in console,
                                    //TODO check if this needs parsing
                                    Console.WriteLine("data------------{0}", data);
                                    AdItem adItem = new AdItem(data);
                                    return adItem;
                                }
                                else
                                {
                                    Console.WriteLine("NO Data----------");
                                    return null; //TODO do better than null
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception Hit------------"); //changes this over to seq data logging
                Console.WriteLine(exception);
            }

            return null;
        }
    }
}