using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class AttachmentType : EntityBase
    {
        public string Name { get; set; }
        public string Aliases { get; set; }
        public string CapoedChordCharts { get; set; }
        public string ChordCharts { get; set; }
        public string Exclusions { get; set; }
        public string Lyrics { get; set; }
        public string NumberCharts { get; set; }
        public string NumeralCharts { get; set; }
        public string BuiltIn { get; set; }
        public string AttachmentTypeGroupId { get; set; }
        public AttachmentTypeGroup AttachmentTypeGroup { get; set; }
    }
}