namespace PlanningCenter.Api.Giving
{
    public class BatchGroup : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Description { get; set; }
        public string Committed { get; set; }
        public string TotalCents { get; set; }
        public string TotalCurrency { get; set; }
        public string Status { get; set; }
    }
}