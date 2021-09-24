namespace Orbit.Api.Model
{
    public class UpsertMember
    {
        public Member Member { get; set; }
        public OtherIdentity Identity { get; set; }
        public string Url { get; set; }
    }
}