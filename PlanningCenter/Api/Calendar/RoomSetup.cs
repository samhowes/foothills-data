namespace PlanningCenter.Api.Calendar
{
    public class RoomSetup : EntityBase
    {
        public string CreatedAt { get; set; }
        public string Name { get; set; }
        public string UpdatedAt { get; set; }
        public string Description { get; set; }
        public string Diagram { get; set; }
        public string DiagramUrl { get; set; }
        public string DiagramThumbnailUrl { get; set; }
        public string RoomSetupId { get; set; }
        public RoomSetup Parent { get; set; }
        public string ContainingResourceId { get; set; }
        public Resource ContainingResource { get; set; }
    }
}