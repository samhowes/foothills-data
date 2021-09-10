namespace PlanningCenter.Api.Services
{
    public class TeamPosition : EntityBase
    {
        public string Name { get; set; }
        public string Tags { get; set; }
        public string NegativeTagGroups { get; set; }
        public string TagGroups { get; set; }
    }
}