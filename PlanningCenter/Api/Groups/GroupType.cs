namespace PlanningCenter.Api.Groups
{
    public class GroupType : EntityBase
    {
        public string ChurchCenterVisible { get; set; }
        public string ChurchCenterMapVisible { get; set; }
        public string Color { get; set; }
        public string DefaultGroupSettings { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
    }
}