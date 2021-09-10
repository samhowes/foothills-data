namespace PlanningCenter.Api.Services
{
    public class ReportTemplate : EntityBase
    {
        public string Body { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
    }
}