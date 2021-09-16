using System;
using System.Collections.Generic;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json.Linq;

namespace JsonApi
{
    public static class DictionaryExtensions
    {
        public static TValue? SafeGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            dict.TryGetValue(key, out var value);
            return value;
        }
    }
    public static class LinksExtensions
    {
        public static string Self(this Links links) => links.SafeGet("self")!.Href;
        public static string? Next(this Links links) => links.SafeGet("next")?.Href;
        public static string? Prev(this Links links) => links.SafeGet("prev")?.Href;
    }

    public static class MetaExtensions
    {
        public static long GetInt(this Meta meta, string key) => (long) ((JValue) meta[key]).Value!;
        public static int PageCount(this Meta meta) => (int)Math.Ceiling(TotalCount(meta) / (decimal)Math.Max(Count(meta), 1L));
        public static long TotalCount(this Meta meta) => meta.GetInt("total_count");

        public static long Count(this Meta meta) => meta.GetInt("count");
    }
}