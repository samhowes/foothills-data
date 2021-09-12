using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using Serilog;
using Constants = PlanningCenter.Api.Constants;

namespace Sync
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var peopleClient = PlanningCenterClient.Create(Constants.PeoplePrefix);
            // var checkinsClient = PlanningCenterClient.Create(Constants.CheckInsPrefix);
            var orbitClient = OrbitApiClient.Create("");
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            
            var workspaceSlug = "sam-workspace";
            var logDb = await GetLog();

            var deps = new SyncDependencies(peopleClient, logDb, orbitClient, workspaceSlug, log);
            var sync = new Synchronizer(log, deps);
            
            log.Information("Starting sync to workspace {WorkspaceSlug}...", workspaceSlug);
            await sync.PeopleToMembers();
        }

        private static async Task<LogDbContext> GetLog()
        {
            var root = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(root!, ".git")))
                root = Path.GetDirectoryName(root)!;
            var dbPath = Path.Combine(root, "log.db");

            var log = new LogDbContext(new DbContextOptionsBuilder<LogDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options);
            await log.Database.EnsureCreatedAsync();
            return log;
        }
    }
}
