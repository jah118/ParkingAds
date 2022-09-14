﻿using pullADs.util;
using System;
using System.Threading.Tasks;
using System.Timers;



namespace pullADs
{
    internal class Program
    {
        private static System.Timers.Timer aTimer;

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //setups
            var startupVars = new Startup();  // add interval timer var to json 
            AdPullService adPullService = new AdPullService();

            DataHandling(startupVars, adPullService);

            // The code runs when a event from OnTimedEvent in SetTimer gets raised.
            // which is delegate that runs from an event from Timer.Elapsed
            SetTimer(startupVars.ApiSettings.TimerInterval1min);

            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            Console.WriteLine("Press 'S' to stop");

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S))
            {
                // do something
            }

            aTimer.Stop();
            aTimer.Dispose();

            Console.WriteLine("Terminating the application...");

        }

        private static void SetTimer(int interval)
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(interval);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static async Task DataHandling(Startup startupVars, AdPullService adPullService)
        {
            Console.WriteLine("pull");

            await adPullService!.GetAd(startupVars.ApiSettings.AdURL);

            Console.WriteLine("data formating ");

            Console.WriteLine("rabbit send ");
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                e.SignalTime);
        }
    }
}