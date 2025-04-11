using System;
using System.Diagnostics;

namespace OWOGame.Infraestructure
{
    public class RealTimeClock
    {
        readonly Stopwatch stopwatch;

        public RealTimeClock()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public long TotalMilliseconds => (long)stopwatch.Elapsed.TotalMilliseconds;
    }
}
