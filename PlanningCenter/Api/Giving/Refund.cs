namespace PlanningCenter.Api.Giving
{
    public class Refund
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string AmountCents { get; set; }
        public string AmountCurrency { get; set; }
        public string FeeCents { get; set; }
        public string RefundedAt { get; set; }
        public string FeeCurrency { get; set; }
    }
}