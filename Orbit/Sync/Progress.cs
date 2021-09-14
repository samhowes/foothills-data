using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Sync
{
    public class Progress : IDisposable
    {
        public Progress()
        {
            Timer = new Stopwatch();
        }
        
        [Key]
        public int? Id { get; set; }
        public string? Type { get; set; }
        public string? NextUrl { get; set; }
        
        [NotMapped]
        public Stopwatch Timer { get; }
        public int Skipped { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        
        [NotMapped]
        public int Total => Skipped + Success + Failed;
        [NotMapped]
        public decimal SecondsElapsed => Timer.ElapsedMilliseconds / 1000m;
        [NotMapped]
        public decimal RecordsPerSecond => ((decimal)Total / Math.Max(Timer.ElapsedMilliseconds, 1)) * 1000m;

        public long TotalTime { get; set; }

        public void Accumulate(Progress other)
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