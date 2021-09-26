using System;
using System.Collections;
using System.Collections.Generic;
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

        public Progress(string type, string nextUrl) : this()
        {
            Type = type;
            NextUrl = nextUrl;
        }
        
        [Key]
        public int? Id { get; set; }
        [ForeignKey(nameof(Parent))]
        public int? ParentId { get; set; }
        public virtual Progress Parent { get; set; }
        public string Type { get; set; }
        public string? NextUrl { get; set; }
        
        [NotMapped]
        public Stopwatch Timer { get; }
        public int Skipped { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public virtual ICollection<Progress> Children { get; set; } = new HashSet<Progress>();

        [NotMapped]
        public int Total => Skipped + Success + Failed;
        [NotMapped]
        public decimal SecondsElapsed => Timer.ElapsedMilliseconds / 1000m;
        [NotMapped]
        public decimal RecordsPerSecond => ((decimal)Total / Math.Max(Timer.ElapsedMilliseconds, 1)) * 1000m;

        public long TotalTime { get; set; }
        public bool Complete { get; set; }

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

        public void RecordItem(SyncStatus result)
        {
            switch (result)
            {
                case SyncStatus.Ignored:
                    Skipped++;
                    break;
                case SyncStatus.Exists:
                    Skipped++;
                    break;
                case SyncStatus.Failed:
                    Failed++;
                    break;
                case SyncStatus.Success:
                    Success++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}