namespace Orbit.Api.Model
{
    public class UpsertMember
    {
        public string Bio { get; set; }
        public string Birthday { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string TagsToAdd { get; set; }
        public string Email { get; set; }
        public OtherIdentity Identity { get; set; }
        public string Url { get; set; }
    }
}