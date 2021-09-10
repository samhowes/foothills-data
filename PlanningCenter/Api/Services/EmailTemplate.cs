namespace PlanningCenter.Api.Services
{
    public class EmailTemplate : EntityBase
    {
        public string Kind { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string HtmlBody { get; set; }
        public string Subject { get; set; }
    }
}