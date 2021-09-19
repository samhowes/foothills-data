using System.Collections.Generic;
using System.Threading.Tasks;
using JsonApiSerializer.JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.Giving;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sync
{
    public record DonationsConfig 
    {
        public string ActivityType { get; set; } = null!;
        public HashSet<string> ExcludedFundIds { get; set; } = null!;
        public string Channel { get; set; } = null!;
        public decimal Weight { get; set; }
    }

    public class DonationsToActivitiesSync : Sync<Donation>
    {
        private readonly GivingClient _givingClient;
        private readonly DonationsConfig _donationsConfig;

        public DonationsToActivitiesSync(
            GivingClient givingClient, DonationsConfig donationsConfig, SyncDeps deps)
            : base(deps, givingClient)
        {
            _givingClient = givingClient;
            _donationsConfig = donationsConfig;
        }

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
                if (_donationsConfig.ExcludedFundIds?.Contains(designation.Fund.Id!) == true)
                {
                    Deps.Log.Debug("Skipping Fund {FundId}", designation.Fund.Id);
                    progress.Skipped++;
                    continue;
                }

                var fund = await Deps.Cache.GetOrAddEntity(designation.Fund.Id!, async (fundId) =>
                {
                    var fundDocument = await _givingClient.GetAsync<Fund>($"funds/{fundId}");
                    return fundDocument.Data;
                });

                var activity = new UploadActivity(
                    _donationsConfig.Channel,
                    _donationsConfig.ActivityType,
                    OrbitUtil.ActivityKey(designation),
                    donation.ReceivedAt,
                    _donationsConfig.Weight,
                    $"Donated to {fund.Name}",
                    PlanningCenterUtil.DonationLink(donation),
                    "Donation"
                );
                
                await UploadActivity(progress, designation, activity, donation.Person.Id!);
                if (progress.Complete) return;
            }
        }
    }
}