using System.Collections.Generic;
using System.Threading.Tasks;
using JsonApi;
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

    public class DonationsToActivitiesSync : ISync<Donation>
    {
        private readonly GivingClient _givingClient;
        private readonly DonationsConfig _donationsConfig;
        private readonly SyncDeps _deps;
        private SyncContext _context = null!;

        public DonationsToActivitiesSync(
            GivingClient givingClient, DonationsConfig donationsConfig, SyncDeps deps)
        {
            _givingClient = givingClient;
            _donationsConfig = donationsConfig;
            _deps = deps;
        }

        public Task<ApiCursor<Donation>> InitializeAsync(SyncContext context)
        {
            _context = context;
            var url = _context.NextUrl ?? UrlUtil.MakeUrl("donations",
                ("include", "designations"),
                ("order", "-created_at"),
                ("succeeded", "true"));

            return Task.FromResult(new ApiCursor<Donation>(_givingClient, url));
        }

        public async Task<SyncStatus> ProcessItemAsync(Donation donation)
        {
            var progress = _context.BatchProgress;
            if (donation.Refunded)
            {
                _deps.Log.Debug("Ignoring refunded donation {DonationId}", donation.Id);
                return SyncStatus.Ignored;
            }

            if (donation.Person == null)
            {
                _deps.Log.Debug("Ignoring donation with no person attached: {DonationLink}, Batch: {BatchId}", 
                    PlanningCenterUtil.DonationLink(donation), donation.Batch?.Id);
                return SyncStatus.Ignored;
            }

            foreach (var designation in donation.Designations.Data)
            {
                if (_donationsConfig.ExcludedFundIds?.Contains(designation.Fund.Id!) == true)
                {
                    _deps.Log.Debug("Skipping Fund {FundId}", designation.Fund.Id);
                    progress.Skipped++;
                    continue;
                }

                var fund = await _deps.Cache.GetOrAddEntity(designation.Fund.Id!, async (fundId) =>
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
                
                var status = await _deps.OrbitSync.UploadActivity<Donation, Designation>(
                    designation, activity, donation.Person.Id!);
                if (status != SyncStatus.Success) return status;
                _context.BatchProgress.RecordItem(status);
            }

            _context.BatchProgress.Success--;
            return SyncStatus.Success;
        }
    }
}