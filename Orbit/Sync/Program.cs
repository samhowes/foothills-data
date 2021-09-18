using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Api;
using PlanningCenter.Api;
using SamHowes.Extensions.Configuration.Yaml;
using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sync
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var workspaceSlug = "sam-workspace";
            
            var services = new ServiceCollection();
            services
                .AddPlanningCenter()
                .AddOrbitApi(workspaceSlug);

            var root = FindRoot();

            services.AddSingleton(new FilesConfig() {Root = root});
            
            var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(root, "sync.log"))
                .CreateLogger();
            services.AddSingleton<ILogger>(log);

            services.AddSingleton<Synchronizer>();
            
            services.AddTransient<SyncDeps>();
            
            services.AddSingleton<DataCache>();
            services.AddTransient<PeopleToMembersSync>();
            services.AddTransient<CheckInsToActivitiesSync>();
            services.AddTransient<DonationsToActivitiesSync>();

            LoadConfigs(services,
                typeof(CheckInsConfig),
                typeof(GroupsConfig),
                typeof(NotesConfig));
            
            var db = await GetLog(root);
            services.AddSingleton(db);
            
            services.AddTransient<GroupAttendanceSync>();
            
            services.AddTransient<MembershipSync>();

            services.AddTransient<NotesToActivitiesSync>();

            services.AddSingleton(new SyncConfig(0));
            
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
                await sync.CheckInsToActivities();
                // await sync.DonationsToActivities();
                
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

        private static void LoadConfigs(ServiceCollection services, params Type[] types)
        {
            // assume we're in the bin somewhere
            var dir = Directory.GetCurrentDirectory();
            var bin = dir!.IndexOf("bin", StringComparison.Ordinal);
            var projectDir = dir[0..(bin - 1)];
            var configuration = new ConfigurationBuilder()
                .AddYamlFile(Path.Combine(projectDir, "sync.yaml"))
                .Build();

            var baseSection = configuration.GetSection("sync");

            foreach (var type in types)
            {
                var section = baseSection.GetSection(type.Name);
                if (!section.Exists())
                    throw new Exception($"Could not find section for {type.Name} in configuration");
                var config = section.Get(type);
                
                if (config is IPostProcessConfig postProcessor)
                    postProcessor.PostProcess();
                services.AddSingleton(type, config);
            }
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
            
            if (true)
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

    public class FilesConfig
    {
        public string Root { get; set; }
    }
}