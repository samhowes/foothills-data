using JsonApi;

namespace PlanningCenter.Api.People
{
    public class WorkflowCardActivity : EntityBase
    {
        public string Comment { get; set; }
        public string Content { get; set; }
        public string FormSubmissionUrl { get; set; }
        public string PersonAvatarUrl { get; set; }
        public string PersonName { get; set; }
        public string ReassignedToAvatarUrl { get; set; }
        public string ReassignedToName { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public string ContentIsHtml { get; set; }
        public string CreatedAt { get; set; }
        public string WorkflowCardId { get; set; }
        public WorkflowCard WorkflowCard { get; set; }
    }
}