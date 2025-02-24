namespace Game.Logic
{
    using System;
    using System.Diagnostics;

    public static class TickHelper
    {
        private static long StopwatchFrequencyMilliseconds = (Stopwatch.Frequency / 0x3e8L);

        public static long GetTickCount()
        {
            return (Stopwatch.GetTimestamp() / StopwatchFrequencyMilliseconds);
        }
    }
}

