namespace PlanningCenter.Api.Giving
{
    public class RecurringDonation : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ReleaseHoldAt { get; set; }
        public string AmountCents { get; set; }
        public string Status { get; set; }
        public string LastDonationReceivedAt { get; set; }
        public string NextOccurrence { get; set; }
        public string Schedule { get; set; }
        public string AmountCurrency { get; set; }
    }
}