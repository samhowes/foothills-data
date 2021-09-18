using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class EmailTemplateRenderedResponse : EntityBase
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string EmailTemplateId { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
    }
}