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
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;


namespace Orbit.Api.Model
{
    /// <summary>
    /// ActivityAndIdentity
    /// </summary>
    [DataContract]
        public partial class ActivityAndIdentity :  IEquatable<ActivityAndIdentity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAndIdentity" /> class.
        /// </summary>
        /// <param name="activity">activity.</param>
        /// <param name="identity">identity.</param>
        public ActivityAndIdentity(OneOfactivityAndIdentityActivity activity = default(OneOfactivityAndIdentityActivity), OtherIdentity identity = default(OtherIdentity))
        {
            this.Activity = activity;
            this.Identity = identity;
        }
        
        /// <summary>
        /// Gets or Sets Activity
        /// </summary>
        [DataMember(Name="activity", EmitDefaultValue=false)]
        public OneOfactivityAndIdentityActivity Activity { get; set; }

        /// <summary>
        /// Gets or Sets Identity
        /// </summary>
        [DataMember(Name="identity", EmitDefaultValue=false)]
        public OtherIdentity Identity { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ActivityAndIdentity {\n");
            sb.Append("  Activity: ").Append(Activity).Append("\n");
            sb.Append("  Identity: ").Append(Identity).Append("\n");
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
            return this.Equals(input as ActivityAndIdentity);
        }

        /// <summary>
        /// Returns true if ActivityAndIdentity instances are equal
        /// </summary>
        /// <param name="input">Instance of ActivityAndIdentity to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ActivityAndIdentity input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Activity == input.Activity ||
                    (this.Activity != null &&
                    this.Activity.Equals(input.Activity))
                ) && 
                (
                    this.Identity == input.Identity ||
                    (this.Identity != null &&
                    this.Identity.Equals(input.Identity))
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
                if (this.Activity != null)
                    hashCode = hashCode * 59 + this.Activity.GetHashCode();
                if (this.Identity != null)
                    hashCode = hashCode * 59 + this.Identity.GetHashCode();
                return hashCode;
            }
        }
    }
}
