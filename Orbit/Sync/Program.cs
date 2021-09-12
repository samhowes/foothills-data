using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using Serilog;

namespace Sync
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddPlanningCenter();
            
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            services.AddSingleton<ILogger>(log);
            services.AddSingleton(await GetLog());
            
            var workspaceSlug = "sam-workspace";

            services.AddSingleton<Synchronizer>();
            services.AddTransient<PeopleToMembersSync>();
            services.AddTransient<CheckInsToActivitiesSync>();

            var provider = services.BuildServiceProvider();
            var sync = provider.GetRequiredService<Synchronizer>();

            log.Information("Starting sync to workspace {WorkspaceSlug}...", workspaceSlug);
            // await sync.PeopleToMembers();
            await sync.CheckInsToActivities();
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
