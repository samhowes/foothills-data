namespace PlanningCenter.Api.Services
{
    public class ItemTime : EntityBase
    {
        public string LiveStartAt { get; set; }
        public string LiveEndAt { get; set; }
        public string Exclude { get; set; }
        public string Length { get; set; }
        public string LengthOffset { get; set; }
    }
}