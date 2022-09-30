using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace pullADs.util
{
    //https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer?view=net-6.0
    public class CTimer
    {
        private static System.Timers.Timer _aTimer;

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            _aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 


            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.AutoReset = true;
            _aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                e.SignalTime);
        }
    }
}
