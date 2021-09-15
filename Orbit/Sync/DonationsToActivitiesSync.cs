using System.Collections.Generic;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.Giving;
using Serilog;

namespace Sync
{
    public class PlanningCenterCache
    {
        public Dictionary<string, Fund> Funds { get; } = new Dictionary<string, Fund>();
    }
    public class DonationsToActivitiesSync : Sync<Donation>
    {
        private readonly GivingClient _givingClient;
        private readonly DonationsConfig _config;
        private readonly ILogger _log;
        private readonly PlanningCenterCache _cache = new();

        public DonationsToActivitiesSync(
            GivingClient givingClient, DonationsConfig config, ILogger log, OrbitApiClient orbitClient, 
            LogDbContext logDb, DataCache cache)
            : base(config, orbitClient, cache, log, logDb, givingClient)
        {
            _givingClient = givingClient;
            _config = config;
            _log = log;
        }

        public override string From => "Donations";
        public override string To => "Activities";
        
        public override async Task<DocumentRoot<List<Donation>>> GetInitialDataAsync(string? nextUrl)
        {
            if (nextUrl != null)
            {
                return await _givingClient.GetAsync<List<Donation>>(nextUrl);
            }
            else
            {
                return  await _givingClient.GetAsync<List<Donation>>("donations",
                    ("include", "designations"),
                    ("order", "created_at"),
                    ("succeeded", "true"));
            }
        }

        public override async Task ProcessBatchAsync(Progress progress, Donation donation)
        {
            if (donation.Refunded)
            {
                progress.Skipped++;
                return;
            }

            foreach (var designation in donation.Designations.Data)
            {
                if (_config.ExcludedFundIds?.Contains(designation.Fund.Id) == true)
                {
                    _log.Debug("Skipping Fund {FundId}", designation.Fund.Id);
                    progress.Skipped++;
                    continue;
                }

                if (!_cache.Funds.TryGetValue(designation.Fund.Id, out var fund))
                {
                    var fundDocument = await _givingClient.GetAsync<Fund>($"funds/{designation.Fund.Id}");
                    fund = fundDocument.Data;
                    _cache.Funds[fund.Id] = fund;
                }

                var activity = new UploadActivity()
                {
                    Title = $"Donated to {fund.Name}",
                    Key = OrbitUtil.ActivityKey(designation),
                    Link = donation.Links.Self(),
                    // todo(#16) get the app link to a donation
                    // $"https://check-ins.planningcenteronline.com/event_periods/{checkIn.EventPeriod.Id}/check_ins/{checkIn.Id}",
                    LinkText = "Donation",
                    ActivityType = "Donation",
                    OccurredAt = donation.CreatedAt,
                    Tags = new List<string>()
                    {
                        "channel:Giving"
                    },
                    Weight = "2"
                };

                if (donation.Person == null)
                {
                    _log.Debug("Ignoring donation with no person attached: {DonationLink}", donation.Links.Self());
                    progress.Skipped++;
                    continue;
                }

                await UploadActivity(progress, donation, activity, donation.Person.Id);
            }

        }
    }
}