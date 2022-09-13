using pullADs.util;
using System;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;


namespace pullADs
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var startup = new Startup();
            
            Timer timer = new Timer();



            AdPullService adPullService = new AdPullService();

            do
            {
            await adPullService!.GetAd(startup.ApiSettings.AdURL);
            } while (b);
        }
        private static Task<bool> HandleTimer()
        {
            bool res  = true;
            return Task.FromResult(res);
        }
    }
}