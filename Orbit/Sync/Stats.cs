using System;
using System.Diagnostics;

namespace Sync
{
    public class Stats : IDisposable
    {
        public Stats()
        {
            Timer = new Stopwatch();
            Timer.Start();
        }
        public Stopwatch Timer { get; }
        public int Skipped { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public int Total => Skipped + Success + Failed;
        public decimal SecondsElapsed => Timer.ElapsedMilliseconds / 1000m;
        public decimal RecordsPerSecond => ((decimal)Total / Math.Max(Timer.ElapsedMilliseconds, 1)) * 1000m;

        public void Accumulate(Stats other)
        {
            Skipped += other.Skipped;
            Success += other.Success;
            Failed += other.Failed;
        }

        public void Dispose()
        {
            Timer.Stop();
        }
    }
}