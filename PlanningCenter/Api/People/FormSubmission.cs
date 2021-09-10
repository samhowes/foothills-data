namespace PlanningCenter.Api.People
{
    public class FormSubmission : EntityBase
    {
        public string Verified { get; set; }
        public string RequiresVerification { get; set; }
        public string CreatedAt { get; set; }
    }
}