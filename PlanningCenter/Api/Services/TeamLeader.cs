using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class TeamLeader : EntityBase
    {
        public string SendResponsesForAccepts { get; set; }
        public string SendResponsesForDeclines { get; set; }
        public string SendResponsesForBlockouts { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
    }
}