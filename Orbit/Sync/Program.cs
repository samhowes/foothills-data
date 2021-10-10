using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.People;
using SamHowes.Extensions.Configuration.Yaml;
using Serilog;

namespace Sync
{
    public class FilesConfig
    {
        public string Root { get; set; }
    }

    public class SyncCommand
    {
        [Value(0, Required = true)] public string Action { get; set; }
        
        [Option('i', "initial")]
        public bool Initial { get; set; }
        
        [Option('f', "force")]
        public bool Force { get; set; }
        
    }

    public static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);

            var result = parser.ParseArguments<SyncCommand>(args);
            if (result.Errors?.Any() == true)
            {
                var text = HelpText.AutoBuild(result);
                Console.Error.WriteLine(text.ToString());
                return -1;
            }

            var command = result.Value;
            
            var provider = await ConfigureServices(command);

            switch (command.Action)
            {
                case "test":
                    return await VerifyApiAccess(provider);
                case "sync":
                    var peopleConfig = provider.GetRequiredService<PeopleConfig>();
                    peopleConfig.Initial = peopleConfig.Initial || command.Initial;
                    
                    var sync = provider.GetRequiredService<Orchestrator>();
                    return await sync.All();
                case "clean":
                    var cleaner = provider.GetRequiredService<Cleaner>();
                    return await cleaner.Clean(args.Skip(1).ToArray());
                    
                default:
                    Console.WriteLine($"Unknown action '{command.Action}'");
                    return -1;
            }
        }

        private static async Task<int> VerifyApiAccess(IServiceProvider provider)
        {
            var peopleClient = provider.GetRequiredService<PeopleClient>();
            var orbitClient = provider.GetRequiredService<OrbitApiClient>();

            // should not throw
            var org = await peopleClient.GetAsync<Organization>("");
            var workspace = await orbitClient.GetAsync<Workspace>("sam-workspace");
            var config = provider.GetRequiredService<FilesConfig>();
            await File.WriteAllTextAsync(Path.Combine(config.Root, "summary.txt"), "success");
            return 0;
        }

        private static async Task<IServiceProvider> ConfigureServices(SyncCommand syncCommand)
        {
            var services = new ServiceCollection();
            services
                .AddPlanningCenter()
                .AddOrbitApi();

            var root = FindRoot();

            services.AddSingleton(new FilesConfig() {Root = root});

            var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(root, "sync.log"))
                .CreateLogger();

            LoadConfigs(services,
                typeof(PeopleConfig),
                typeof(CheckInsConfig),
                typeof(DonationsConfig),
                typeof(GroupConfig),
                typeof(GroupAttendanceConfig),
                typeof(GroupMembershipConfig),
                typeof(NotesConfig));

            var db = await GetLog(root);
            services
                .AddSingleton<ILogger>(log)
                .AddSingleton(db)
                .AddSingleton<Orchestrator>()
                .AddSingleton<OrbitSync>()
                .AddSingleton<GroupSync>()
                .AddTransient<SyncDeps>()
                .AddSingleton<DataCache>()
                .AddTransient<PeopleToMembersSync>()
                .AddTransient<CheckInsToActivitiesSync>()
                .AddTransient<DonationsToActivitiesSync>()
                .AddTransient<GroupAttendanceSync>()
                .AddTransient<GroupMembershipSync>()
                .AddTransient<NotesToActivitiesSync>()
                .AddSingleton<Cleaner>();
            services
                .Configure<SyncImplConfig>(c =>
                {
                    if (syncCommand.Force)
                    {
                        c.Force = true;
                        c.KeyExistsMode = KeyExistsMode.Skip;
                    }
                });

            var provider = services.BuildServiceProvider();
            return provider;
        }

        private static void LoadConfigs(ServiceCollection services, params Type[] types)
        {
            var dir = Directory.GetCurrentDirectory();
            var bin = dir!.IndexOf("bin", StringComparison.Ordinal);
            
            var projectDir = bin < 0 ? dir : dir[0..(bin - 1)];
            var configuration = new ConfigurationBuilder()
                .AddYamlFile(Path.Combine(projectDir, "sync.yaml"))
                .Build();

            var baseSection = configuration.GetSection("sync");

            services.Configure<SyncImplConfig>(baseSection);

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