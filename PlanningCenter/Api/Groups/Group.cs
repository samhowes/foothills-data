using System.Collections.Generic;
using JsonApi;
using JsonApiSerializer.JsonApi;

namespace PlanningCenter.Api.Groups
{
    public class Image
    {
        public string Thumbnail { get; set;  }
        public string Medium { get; set;  }
        public string Original { get; set;  }
    }
    
    public class Group : EntityBase
    {
        public string ArchivedAt { get; set; }
        public string ContactEmail { get; set; }
        public string CreatedAt { get; set; }
        public string Description { get; set; }
        public string EnrollmentOpen { get; set; }
        public string EnrollmentStrategy { get; set; }
        public string EventsVisibility { get; set; }
        public Image HeaderImage { get; set; }
        public string LocationTypePreference { get; set; }
        public string MembershipsCount { get; set; }
        public string Name { get; set; }
        public string PublicChurchCenterWebUrl { get; set; }
        public string Schedule { get; set; }
        public string VirtualLocationUrl { get; set; }
        public GroupType GroupType { get; set; }
        public Location Location { get; set; }
    }
}