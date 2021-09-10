namespace PlanningCenter.Api.CheckIns
{
    public class Pass : EntityBase
    {
        public string Code { get; set; }
        public string Kind { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}