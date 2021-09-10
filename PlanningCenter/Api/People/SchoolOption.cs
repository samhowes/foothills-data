namespace PlanningCenter.Api.People
{
    public class SchoolOption : EntityBase
    {
        public string Value { get; set; }
        public string Sequence { get; set; }
        public string BeginningGrade { get; set; }
        public string EndingGrade { get; set; }
        public string SchoolTypes { get; set; }
    }
}