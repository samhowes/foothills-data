namespace PlanningCenter.Api.Calendar
{
    public class ResourceSuggestion : EntityBase
    {
        public string CreatedAt { get; set; }
        public string Quantity { get; set; }
        public string UpdatedAt { get; set; }
        public string ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string RoomSetupId { get; set; }
        public RoomSetup RoomSetup { get; set; }
    }
}