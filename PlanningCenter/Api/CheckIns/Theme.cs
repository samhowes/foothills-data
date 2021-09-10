namespace PlanningCenter.Api.CheckIns
{
    public class Theme : EntityBase
    {
        public string ImageThumbnail { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string Image { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string BackgroundColor { get; set; }
        public string Mode { get; set; }
    }
}