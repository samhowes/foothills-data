using System;
using JsonApiSerializer.JsonApi;

namespace JsonApi
{
    public static class LinksExtensions
    {
        private static string? SafeGet(Links links, string key)
        {
            links.TryGetValue(key, out var value);
            return value?.Href;
        }
        public static string? Self(this Links links) => SafeGet(links, "self")!;
        public static string? Next(this Links links) => SafeGet(links, "next");
        public static string? Prev(this Links links) => SafeGet(links, "prev");
    }

    public static class MetaExtensions
    {
        public static int PageCount(this Meta meta) => (int)Math.Ceiling(TotalCount(meta) / (decimal)Count(meta));
        public static int TotalCount(this Meta meta) => 0;

        public static int Count(this Meta meta) => 0;
    }
}