using JsonApi;

namespace PlanningCenter.Api.People
{
    public class FormSubmission : EntityBase
    {
        public string Verified { get; set; }
        public string RequiresVerification { get; set; }
        public string CreatedAt { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string FormId { get; set; }
        public Form Form { get; set; }
    }
}