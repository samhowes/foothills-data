using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
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
            
            services.AddTransient<SyncDeps>();
            
            services.AddSingleton<DataCache>();
            services.AddTransient<PeopleToMembersSync>();
            services.AddTransient<CheckInsToActivitiesSync>();
            services.AddTransient<DonationsToActivitiesSync>();

            services.AddSingleton(new GroupsConfig());
            services.AddTransient<GroupAttendanceSync>();
            
            services.AddTransient<MembershipSync>();

            services.AddSingleton(new NotesConfig());
            services.AddTransient<NotesToActivitiesSync>();

            services.AddSingleton(new SyncConfig(0));
            
            // todo(#15) remove this config
            services.AddSingleton(new CheckInsToActivitiesSyncConfig());
            services.AddSingleton(new CheckInsToActivitiesSyncConfig(
                @params: ("order", "-created_at")));

            services.AddSingleton(new DonationsConfig()
            {
                ExcludedFundIds = new HashSet<string>(new[]
                {
                    "34038", "207309", "63896", "181654", "178986", "158390", "156026", "64218", "146161"
                })
            });

            services.AddSingleton(new SyncImplConfig(SyncMode.Update));
            
            var provider = services.BuildServiceProvider();
            var sync = provider.GetRequiredService<Synchronizer>();

            log.Information("Starting sync to workspace {WorkspaceSlug}...", workspaceSlug);
            try
            {
                // await sync.PeopleToMembers();
                // await sync.CheckInsToActivities();
                await sync.DonationsToActivities();
                
                // await sync.GroupAttendanceToActivities();
                // await sync.GroupToActivities();
                
                // await sync.NotesToActivities();
                
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
            var options = new DbContextOptionsBuilder<LogDbContext>();
            
            if (false)
            {
                var connection = new SqliteConnection("Data Source=:memory:");
                await connection.OpenAsync();
                options.UseSqlite(connection);
            }
            else
            {
                var dbPath = Path.Combine(root, "log.db");
                options.UseSqlite($"Data Source={dbPath}");
            }
            
            var log = new LogDbContext(options.Options);
            await log.Database.EnsureCreatedAsync();
            
            return log;
        }
    }
}