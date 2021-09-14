using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Api;
using PlanningCenter.Api;
using Serilog;

namespace Sync
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var workspaceSlug = "sam-workspace";
            var root = FindRoot();
            var services = new ServiceCollection();
            services
                .AddPlanningCenter()
                .AddOrbitApi(workspaceSlug);

            var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(root, "sync.log"))
                .CreateLogger();
            services.AddSingleton<ILogger>(log);
            var db = await GetLog(root);
            services.AddSingleton(db);

            services.AddSingleton<Synchronizer>();
            services.AddTransient<PeopleToMembersSync>();
            services.AddTransient<CheckInsToActivitiesSync>();

            var provider = services.BuildServiceProvider();
            var sync = provider.GetRequiredService<Synchronizer>();

            log.Information("Starting sync to workspace {WorkspaceSlug}...", workspaceSlug);
            try
            {
                // await sync.PeopleToMembers();
                await sync.CheckInsToActivities();
            }
            catch (Exception e)
            {
                log.Fatal(e, "Unexpected error while performing sync");
            }

            await db.SaveChangesAsync();
        }

        private static string FindRoot()
        {
            var root = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(root!, ".git")))
                root = Path.GetDirectoryName(root)!;
            return root;
        }
        
        private static async Task<LogDbContext> GetLog(string root)
        {
            
            var dbPath = Path.Combine(root, "log.db");

            var log = new LogDbContext(new DbContextOptionsBuilder<LogDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options);
            await log.Database.EnsureCreatedAsync();
            return log;
        }
    }
}