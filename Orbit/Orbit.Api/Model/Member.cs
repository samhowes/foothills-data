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
using System.Collections.Generic;
using JsonApi;
using JsonApiSerializer.JsonApi;


namespace Orbit.Api.Model
{
    public partial class Member : EntityBase
    {
        public Member()
        {
        }


        public string Bio { get; set; }


        public string Birthday { get; set; }


        public string Company { get; set; }

        public string Title { get; set; }


        public string Location { get; set; }


        public string? Name { get; set; }


        public string Pronouns { get; set; }

        public string ShippingAddress { get; set; }

        public string Slug { get; set; }

        public string TagsToAdd { get; set; }

        public List<string> TagList { get; set; }
        public string Tshirt { get; set; }

        public bool? Teammate { get; set; }

        public string Url { get; set; }

        public string Github { get; set; }

        public string Twitter { get; set; }

        public string Email { get; set; }

        public string Linkedin { get; set; }

        public string Devto { get; set; }

        public DateTime CreatedAt { get; set; }

        public Relationship<List<OtherIdentity>> Identities { get; set; }
    }
}