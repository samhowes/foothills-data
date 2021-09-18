using JsonApi;

namespace PlanningCenter.Api.People
{
    public class PersonMerger : EntityBase
    {
        public string CreatedAt { get; set; }
        public string PersonToKeepId { get; set; }
        public Person PersonToKeep { get; set; }
        public string PersonToRemoveId { get; set; }
        public Person PersonToRemove { get; set; }
    }
}