using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

namespace JsonApi
{
    public abstract class Resource
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public Links Links { get; set; }
    }
    
    public class Resource<T> : Resource
    {
        public T Attributes { get; set; }
    }

    public class GenericResource : Resource
    {
        public Dictionary<string, string> Attributes { get; set; }
    }

    public class Response
    {
        public Links Links { get; set; }
        
        public Metadata Meta { get; set; }
    }
    
    public class Response<T> : Response
    {
        public T Data { get; set; } 
    }
 
    public class Next
    {
        public int Offset { get; set; }
    }

    public class Parent
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }

    public class Metadata
    {
        public int PageCount => (int)Math.Ceiling(TotalCount / (decimal)Count);
        public int TotalCount { get; set; }
        public int Count { get; set; }
        public Next Next { get; set; }
        public List<string> CanOrderBy { get; set; }
        public List<string> CanQueryBy { get; set; }
        public List<string> CanInclude { get; set; }
        public List<string> CanFilter { get; set; }
        public Parent Parent { get; set; }
    }

    public class Links : Dictionary<string, string>
    {
        private string? SafeGet(string key)
        {
            TryGetValue(key, out var value);
            return value;
        }

        public string Self => SafeGet("self")!;
        public string? Next => SafeGet("next");
        public string? Prev => SafeGet("prev");
    }
}