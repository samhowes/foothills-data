using System.Collections.Generic;
using System.Threading.Tasks;
using JsonApiSerializer.JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.Giving;

namespace Sync
{
    public static class ActivityType
    {
        public const string Donation = "Donation";
    }
    public record DonationsConfig : SyncImplConfig
    {
        public HashSet<string>? ExcludedFundIds { get; init; }
    }

    public class PlanningCenterCache
    {
        public Dictionary<string, Fund> Funds { get; } = new();
    }

    public class DonationsToActivitiesSync : Sync<Donation>
    {
        private readonly GivingClient _givingClient;
        private readonly DonationsConfig _config;
        private readonly PlanningCenterCache _cache = new();

        public DonationsToActivitiesSync(
            GivingClient givingClient, DonationsConfig config, SyncDeps deps)
            : base(deps, givingClient)
        {
            _givingClient = givingClient;
            _config = config;
        }

        protected override List<ActivityMapping> ActivityTypes { get; } = new()
        {
            new ActivityMapping<Designation>(ActivityType.Donation)
        };

        public override async Task<DocumentRoot<List<Donation>>> GetInitialDataAsync(string? nextUrl)
        {
            await base.GetInitialDataAsync(nextUrl);
            
            if (nextUrl != null)
            {
                return await _givingClient.GetAsync<List<Donation>>(nextUrl);
            }

            return await _givingClient.GetAsync<List<Donation>>("donations",
                ("include", "designations"),
                ("order", "-created_at"),
                ("succeeded", "true"));
        }

        public override async Task ProcessBatchAsync(Progress progress, Donation donation)
        {
            if (donation.Refunded)
            {
                Deps.Log.Debug("Ignoring refunded donation {DonationId}", donation.Id);
                progress.Skipped++;
                return;
            }

            if (donation.Person == null)
            {
                Deps.Log.Debug("Ignoring donation with no person attached: {DonationLink}, Batch: {BatchId}", 
                    PlanningCenterUtil.DonationLink(donation), donation.Batch?.Id);
                progress.Skipped++;
                return;
            }

            foreach (var designation in donation.Designations.Data)
            {
                if (_config.ExcludedFundIds?.Contains(designation.Fund.Id!) == true)
                {
                    Deps.Log.Debug("Skipping Fund {FundId}", designation.Fund.Id);
                    progress.Skipped++;
                    continue;
                }

                if (!_cache.Funds.TryGetValue(designation.Fund.Id!, out var fund))
                {
                    var fundDocument = await _givingClient.GetAsync<Fund>($"funds/{designation.Fund.Id}");
                    fund = fundDocument.Data;
                    _cache.Funds[fund.Id!] = fund;
                }

                
                var activity = new UploadActivity(
                    "Giving",
                    "Donation",
                    OrbitUtil.ActivityKey(designation),
                    donation.ReceivedAt,
                    2m,
                    $"Donated to {fund.Name}",
                    PlanningCenterUtil.DonationLink(donation),
                    "Donation"
                );
                
                await UploadActivity(progress, designation, activity, donation.Person.Id!);
            }
        }
    }
}