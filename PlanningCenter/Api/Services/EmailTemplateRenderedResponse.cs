namespace PlanningCenter.Api.Services
{
    public class EmailTemplateRenderedResponse : EntityBase
    {
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}