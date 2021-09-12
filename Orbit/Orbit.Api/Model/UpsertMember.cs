namespace Orbit.Api.Model
{
    public class UpsertMember
    {
        public string Birthday { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string TagsToAdd { get; set; }
        public Identity Identity { get; set; }
    }
}