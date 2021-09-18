using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class ItemTime : EntityBase
    {
        public string LiveStartAt { get; set; }
        public string LiveEndAt { get; set; }
        public string Exclude { get; set; }
        public string Length { get; set; }
        public string LengthOffset { get; set; }
        public string ItemId { get; set; }
        public Item Item { get; set; }
        public string PlanTimeId { get; set; }
        public PlanTime PlanTime { get; set; }
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
    }
}