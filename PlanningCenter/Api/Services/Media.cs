using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class Media : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Themes { get; set; }
        public string Title { get; set; }
        public string ThumbnailFileName { get; set; }
        public string ThumbnailContentType { get; set; }
        public string ThumbnailFileSize { get; set; }
        public string ThumbnailUpdatedAt { get; set; }
        public string PreviewFileName { get; set; }
        public string PreviewContentType { get; set; }
        public string PreviewFileSize { get; set; }
        public string PreviewUpdatedAt { get; set; }
        public string Length { get; set; }
        public string MediaType { get; set; }
        public string MediaTypeName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string CreatorName { get; set; }
        public string PreviewUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}