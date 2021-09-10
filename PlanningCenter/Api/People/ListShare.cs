namespace PlanningCenter.Api.People
{
    public class ListShare : EntityBase
    {
        public string Permission { get; set; }
        public string Group { get; set; }
        public string CreatedAt { get; set; }
        public string Name { get; set; }
    }
}