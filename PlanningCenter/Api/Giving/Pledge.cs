using JsonApi;

namespace PlanningCenter.Api.Giving
{
    public class Pledge : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string AmountCents { get; set; }
        public string AmountCurrency { get; set; }
        public string JointGiverAmountCents { get; set; }
        public string DonatedTotalCents { get; set; }
        public string JointGiverDonatedTotalCents { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string PledgeCampaignId { get; set; }
        public PledgeCampaign PledgeCampaign { get; set; }
    }
}