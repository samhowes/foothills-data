/* 
 * Orbit API
 *
 * Please see the complete Orbit API documentation at [https://docs.orbit.love/reference](https://docs.orbit.love/reference).
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Sync;

namespace Orbit.Api.Model
{
    public class ActivityBase
    {
        public string Id { get; set; }
        
        public string Weight { get; set; }
        
        public string Key { get; set; }
        
        public string OccurredAt { get; set; }
        
        public List<string> Tags { get; set; }
    }

    public class CustomActivity : ActivityBase
    {
        public string? Action { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public string? OrbitUrl { get; set; }
        public string? CustomDescription { get; set; }
        public string? CustomLink { get; set; }
        public string? CustomLinkText { get; set; }
        public string? CustomTitle { get; set; }
        public string? CustomType { get; set; }
        public Member Member { get; set; } = null!;
    }
    
    public class UploadActivity : ActivityBase
    {
        public UploadActivity()
        {
            
        }

        public UploadActivity(string channel, string type, string key, string occurredAt, decimal weight, 
            string title, string link, string linkText, params string[]tags)
        {
            // functional fields
            Tags = tags.Concat(new []
            {
                OrbitUtil.ChannelTag(channel),
            }).ToList();
            ActivityType = type;
            Key = key;
            OccurredAt = occurredAt;
            Weight = weight.ToString(CultureInfo.InvariantCulture);

            // display fields
            Title = title;
            Link = link;
            LinkText = linkText;
        }
        public string Type => "custom_activity";
        public string Description { get; set; }


        public string Link { get; set; }


        public string LinkText { get; set; }


        public string Title { get; set; }

        public string ActivityType { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Activity {\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  Link: ").Append(Link).Append("\n");
            sb.Append("  LinkText: ").Append(LinkText).Append("\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  Weight: ").Append(Weight).Append("\n");
            sb.Append("  ActivityType: ").Append(ActivityType).Append("\n");
            sb.Append("  Key: ").Append(Key).Append("\n");
            sb.Append("  OccurredAt: ").Append(OccurredAt).Append("\n");
            sb.Append("  Tags: ").Append(Tags).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

    }
}
