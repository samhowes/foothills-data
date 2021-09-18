using JsonApi;

namespace PlanningCenter.Api.People
{
    public class FormSubmissionValue : EntityBase
    {
        public string DisplayValue { get; set; }
        public string FormFieldId { get; set; }
        public FormField FormField { get; set; }
        public string FormSubmissionId { get; set; }
        public FormSubmission FormSubmission { get; set; }
    }
}