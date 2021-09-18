using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class Location : EntityBase
    {
        public string Name { get; set; }
        public string Kind { get; set; }
        public string Opened { get; set; }
        // public string Questions { get; set; }
        public string AgeMinInMonths { get; set; }
        public string AgeMaxInMonths { get; set; }
        public string AgeRangeBy { get; set; }
        public string AgeOn { get; set; }
        public string ChildOrAdult { get; set; }
        public string EffectiveDate { get; set; }
        public string Gender { get; set; }
        public string GradeMin { get; set; }
        public string GradeMax { get; set; }
        public string MaxOccupancy { get; set; }
        public string MinVolunteers { get; set; }
        public string AttendeesPerVolunteer { get; set; }
        public string Position { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedAt { get; set; }
        public string ParentId { get; set; }
        public Parent Parent { get; set; }
    }
}