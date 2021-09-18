using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;


namespace Orbit.Api.Model
{
    public class OtherIdentity
    {
        public OtherIdentity()
        {
            
        }

        public OtherIdentity(string source)
        {
            Source = source;
        }
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Source { get; set; }
        public string? SourceHost { get; set; }
        public string? Username { get; set; }
        public string? Uid { get; set; }
        public string? Email { get; set; }
        public string? Url { get; set; }
    }
}
