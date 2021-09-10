namespace PlanningCenter.Api.CheckIns
{
    public class CheckInGroup : EntityBase
    {
        public string NameLabelsCount { get; set; }
        public string SecurityLabelsCount { get; set; }
        public string CheckInsCount { get; set; }
        public string PrintStatus { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}