using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Sync
{
    public class Mapping
    {
        [Key]
        public int Id { get; set; }
        public string OrbitId { get; set; }
        [Required]
        public string PlanningCenterId { get; set; }
        public string Data { get; set; }
        public string Error { get; set; }
        public string Type { get; set; }
    }
    
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
            
        }

        public DbSet<Mapping> Mappings { get; set; }    
    }
}