namespace PlanningCenter.Api.Giving
{
    public class Fund : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Name { get; set; }
        public string LedgerCode { get; set; }
        public string Description { get; set; }
        public string Visibility { get; set; }
        public string Default { get; set; }
        public string Color { get; set; }
        public string Deletable { get; set; }
    }
}