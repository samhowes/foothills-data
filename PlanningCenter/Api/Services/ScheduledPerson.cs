using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class ScheduledPerson : EntityBase
    {
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Thumbnail { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string SignupSheetId { get; set; }
        public SignupSheet SignupSheet { get; set; }
    }
}