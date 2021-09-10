namespace PlanningCenter.Api.Giving
{
    public class Designation : EntityBase
    {
        public string AmountCents { get; set; }
        public string AmountCurrency { get; set; }
        public string FundId { get; set; }
        public Fund Fund { get; set; }
    }
}