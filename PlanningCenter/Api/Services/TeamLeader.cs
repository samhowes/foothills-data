namespace PlanningCenter.Api.Services
{
    public class TeamLeader : EntityBase
    {
        public string SendResponsesForAccepts { get; set; }
        public string SendResponsesForDeclines { get; set; }
        public string SendResponsesForBlockouts { get; set; }
    }
}