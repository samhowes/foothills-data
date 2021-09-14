/* 
 * Orbit API
 *
 * Please see the complete Orbit API documentation at [https://docs.orbit.love/reference](https://docs.orbit.love/reference).
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Orbit.Api.Model
{
    /// <summary>
    /// Member
    /// </summary>
    [DataContract]
        public partial class Member :  IEquatable<Member>
    {
        public string Id { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Member" /> class.
        /// </summary>
        /// <param name="bio">bio.</param>
        /// <param name="birthday">birthday.</param>
        /// <param name="company">company.</param>
        /// <param name="title">title.</param>
        /// <param name="location">location.</param>
        /// <param name="name">name.</param>
        /// <param name="pronouns">pronouns.</param>
        /// <param name="shippingAddress">shippingAddress.</param>
        /// <param name="slug">slug.</param>
        /// <param name="tagsToAdd">Adds tags to member; comma-separated string or array.</param>
        /// <param name="tags">Replaces all tags for the member; comma-separated string or array.</param>
        /// <param name="tagList">Deprecated: Please use the tags attribute instead.</param>
        /// <param name="tshirt">tshirt.</param>
        /// <param name="teammate">teammate.</param>
        /// <param name="url">url.</param>
        /// <param name="github">The member&#x27;s GitHub username.</param>
        /// <param name="twitter">The member&#x27;s Twitter username.</param>
        /// <param name="email">The member&#x27;s email.</param>
        /// <param name="linkedin">The member&#x27;s LinkedIn username, without the in/ or pub/.</param>
        /// <param name="devto">The member&#x27;s dev.to username.</param>
        public Member(string bio = default(string), string birthday = default(string), string company = default(string), string title = default(string), string location = default(string), string name = default(string), string pronouns = default(string), string shippingAddress = default(string), string slug = default(string), string tagsToAdd = default(string), List<string> tags = default(List<string>), List<string> tagList = default(List<string>), string tshirt = default(string), bool? teammate = default(bool?), string url = default(string), string github = default(string), string twitter = default(string), string email = default(string), string linkedin = default(string), string devto = default(string))
        {
            this.Bio = bio;
            this.Birthday = birthday;
            this.Company = company;
            this.Title = title;
            this.Location = location;
            this.Name = name;
            this.Pronouns = pronouns;
            this.ShippingAddress = shippingAddress;
            this.Slug = slug;
            this.TagsToAdd = tagsToAdd;
            this.Tags = tags;
            this.TagList = tagList;
            this.Tshirt = tshirt;
            this.Teammate = teammate;
            this.Url = url;
            this.Github = github;
            this.Twitter = twitter;
            this.Email = email;
            this.Linkedin = linkedin;
            this.Devto = devto;
        }
        
        /// <summary>
        /// Gets or Sets Bio
        /// </summary>
        [DataMember(Name="bio", EmitDefaultValue=false)]
        public string Bio { get; set; }

        /// <summary>
        /// Gets or Sets Birthday
        /// </summary>
        [DataMember(Name="birthday", EmitDefaultValue=false)]
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or Sets Company
        /// </summary>
        [DataMember(Name="company", EmitDefaultValue=false)]
        public string Company { get; set; }

        /// <summary>
        /// Gets or Sets Title
        /// </summary>
        [DataMember(Name="title", EmitDefaultValue=false)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or Sets Location
        /// </summary>
        [DataMember(Name="location", EmitDefaultValue=false)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Pronouns
        /// </summary>
        [DataMember(Name="pronouns", EmitDefaultValue=false)]
        public string Pronouns { get; set; }

        /// <summary>
        /// Gets or Sets ShippingAddress
        /// </summary>
        [DataMember(Name="shipping_address", EmitDefaultValue=false)]
        public string ShippingAddress { get; set; }

        /// <summary>
        /// Gets or Sets Slug
        /// </summary>
        [DataMember(Name="slug", EmitDefaultValue=false)]
        public string Slug { get; set; }

        /// <summary>
        /// Adds tags to member; comma-separated string or array
        /// </summary>
        /// <value>Adds tags to member; comma-separated string or array</value>
        [DataMember(Name="tags_to_add", EmitDefaultValue=false)]
        public string TagsToAdd { get; set; }

        /// <summary>
        /// Replaces all tags for the member; comma-separated string or array
        /// </summary>
        /// <value>Replaces all tags for the member; comma-separated string or array</value>
        [DataMember(Name="tags", EmitDefaultValue=false)]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Deprecated: Please use the tags attribute instead
        /// </summary>
        /// <value>Deprecated: Please use the tags attribute instead</value>
        [DataMember(Name="tag_list", EmitDefaultValue=false)]
        public List<string> TagList { get; set; }

        /// <summary>
        /// Gets or Sets Tshirt
        /// </summary>
        [DataMember(Name="tshirt", EmitDefaultValue=false)]
        public string Tshirt { get; set; }

        /// <summary>
        /// Gets or Sets Teammate
        /// </summary>
        [DataMember(Name="teammate", EmitDefaultValue=false)]
        public bool? Teammate { get; set; }

        /// <summary>
        /// Gets or Sets Url
        /// </summary>
        [DataMember(Name="url", EmitDefaultValue=false)]
        public string Url { get; set; }

        /// <summary>
        /// The member&#x27;s GitHub username
        /// </summary>
        /// <value>The member&#x27;s GitHub username</value>
        [DataMember(Name="github", EmitDefaultValue=false)]
        public string Github { get; set; }

        /// <summary>
        /// The member&#x27;s Twitter username
        /// </summary>
        /// <value>The member&#x27;s Twitter username</value>
        [DataMember(Name="twitter", EmitDefaultValue=false)]
        public string Twitter { get; set; }

        /// <summary>
        /// The member&#x27;s email
        /// </summary>
        /// <value>The member&#x27;s email</value>
        [DataMember(Name="email", EmitDefaultValue=false)]
        public string Email { get; set; }

        /// <summary>
        /// The member&#x27;s LinkedIn username, without the in/ or pub/
        /// </summary>
        /// <value>The member&#x27;s LinkedIn username, without the in/ or pub/</value>
        [DataMember(Name="linkedin", EmitDefaultValue=false)]
        public string Linkedin { get; set; }

        /// <summary>
        /// The member&#x27;s dev.to username
        /// </summary>
        /// <value>The member&#x27;s dev.to username</value>
        [DataMember(Name="devto", EmitDefaultValue=false)]
        public string Devto { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Member {\n");
            sb.Append("  Bio: ").Append(Bio).Append("\n");
            sb.Append("  Birthday: ").Append(Birthday).Append("\n");
            sb.Append("  Company: ").Append(Company).Append("\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  Location: ").Append(Location).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Pronouns: ").Append(Pronouns).Append("\n");
            sb.Append("  ShippingAddress: ").Append(ShippingAddress).Append("\n");
            sb.Append("  Slug: ").Append(Slug).Append("\n");
            sb.Append("  TagsToAdd: ").Append(TagsToAdd).Append("\n");
            sb.Append("  Tags: ").Append(Tags).Append("\n");
            sb.Append("  TagList: ").Append(TagList).Append("\n");
            sb.Append("  Tshirt: ").Append(Tshirt).Append("\n");
            sb.Append("  Teammate: ").Append(Teammate).Append("\n");
            sb.Append("  Url: ").Append(Url).Append("\n");
            sb.Append("  Github: ").Append(Github).Append("\n");
            sb.Append("  Twitter: ").Append(Twitter).Append("\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  Linkedin: ").Append(Linkedin).Append("\n");
            sb.Append("  Devto: ").Append(Devto).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as Member);
        }

        /// <summary>
        /// Returns true if Member instances are equal
        /// </summary>
        /// <param name="input">Instance of Member to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Member input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Bio == input.Bio ||
                    (this.Bio != null &&
                    this.Bio.Equals(input.Bio))
                ) && 
                (
                    this.Birthday == input.Birthday ||
                    (this.Birthday != null &&
                    this.Birthday.Equals(input.Birthday))
                ) && 
                (
                    this.Company == input.Company ||
                    (this.Company != null &&
                    this.Company.Equals(input.Company))
                ) && 
                (
                    this.Title == input.Title ||
                    (this.Title != null &&
                    this.Title.Equals(input.Title))
                ) && 
                (
                    this.Location == input.Location ||
                    (this.Location != null &&
                    this.Location.Equals(input.Location))
                ) && 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.Pronouns == input.Pronouns ||
                    (this.Pronouns != null &&
                    this.Pronouns.Equals(input.Pronouns))
                ) && 
                (
                    this.ShippingAddress == input.ShippingAddress ||
                    (this.ShippingAddress != null &&
                    this.ShippingAddress.Equals(input.ShippingAddress))
                ) && 
                (
                    this.Slug == input.Slug ||
                    (this.Slug != null &&
                    this.Slug.Equals(input.Slug))
                ) && 
                (
                    this.TagsToAdd == input.TagsToAdd ||
                    (this.TagsToAdd != null &&
                    this.TagsToAdd.Equals(input.TagsToAdd))
                ) && 
                (
                    this.Tags == input.Tags ||
                    (this.Tags != null &&
                    this.Tags.Equals(input.Tags))
                ) && 
                (
                    this.TagList == input.TagList ||
                    (this.TagList != null &&
                    this.TagList.Equals(input.TagList))
                ) && 
                (
                    this.Tshirt == input.Tshirt ||
                    (this.Tshirt != null &&
                    this.Tshirt.Equals(input.Tshirt))
                ) && 
                (
                    this.Teammate == input.Teammate ||
                    (this.Teammate != null &&
                    this.Teammate.Equals(input.Teammate))
                ) && 
                (
                    this.Url == input.Url ||
                    (this.Url != null &&
                    this.Url.Equals(input.Url))
                ) && 
                (
                    this.Github == input.Github ||
                    (this.Github != null &&
                    this.Github.Equals(input.Github))
                ) && 
                (
                    this.Twitter == input.Twitter ||
                    (this.Twitter != null &&
                    this.Twitter.Equals(input.Twitter))
                ) && 
                (
                    this.Email == input.Email ||
                    (this.Email != null &&
                    this.Email.Equals(input.Email))
                ) && 
                (
                    this.Linkedin == input.Linkedin ||
                    (this.Linkedin != null &&
                    this.Linkedin.Equals(input.Linkedin))
                ) && 
                (
                    this.Devto == input.Devto ||
                    (this.Devto != null &&
                    this.Devto.Equals(input.Devto))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Bio != null)
                    hashCode = hashCode * 59 + this.Bio.GetHashCode();
                if (this.Birthday != null)
                    hashCode = hashCode * 59 + this.Birthday.GetHashCode();
                if (this.Company != null)
                    hashCode = hashCode * 59 + this.Company.GetHashCode();
                if (this.Title != null)
                    hashCode = hashCode * 59 + this.Title.GetHashCode();
                if (this.Location != null)
                    hashCode = hashCode * 59 + this.Location.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.Pronouns != null)
                    hashCode = hashCode * 59 + this.Pronouns.GetHashCode();
                if (this.ShippingAddress != null)
                    hashCode = hashCode * 59 + this.ShippingAddress.GetHashCode();
                if (this.Slug != null)
                    hashCode = hashCode * 59 + this.Slug.GetHashCode();
                if (this.TagsToAdd != null)
                    hashCode = hashCode * 59 + this.TagsToAdd.GetHashCode();
                if (this.Tags != null)
                    hashCode = hashCode * 59 + this.Tags.GetHashCode();
                if (this.TagList != null)
                    hashCode = hashCode * 59 + this.TagList.GetHashCode();
                if (this.Tshirt != null)
                    hashCode = hashCode * 59 + this.Tshirt.GetHashCode();
                if (this.Teammate != null)
                    hashCode = hashCode * 59 + this.Teammate.GetHashCode();
                if (this.Url != null)
                    hashCode = hashCode * 59 + this.Url.GetHashCode();
                if (this.Github != null)
                    hashCode = hashCode * 59 + this.Github.GetHashCode();
                if (this.Twitter != null)
                    hashCode = hashCode * 59 + this.Twitter.GetHashCode();
                if (this.Email != null)
                    hashCode = hashCode * 59 + this.Email.GetHashCode();
                if (this.Linkedin != null)
                    hashCode = hashCode * 59 + this.Linkedin.GetHashCode();
                if (this.Devto != null)
                    hashCode = hashCode * 59 + this.Devto.GetHashCode();
                return hashCode;
            }
        }
    }
}
