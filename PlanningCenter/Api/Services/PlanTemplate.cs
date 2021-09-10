namespace PlanningCenter.Api.Services
{
    public class PlanTemplate : EntityBase
    {
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ItemCount { get; set; }
        public string TeamCount { get; set; }
        public string NoteCount { get; set; }
        public string CanViewOrder { get; set; }
        public string MultiDay { get; set; }
        public string PrefersOrderView { get; set; }
        public string Rehearsable { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        public Person UpdatedBy { get; set; }
    }
}