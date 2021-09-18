using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApiSerializer.JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.People;
using Note = PlanningCenter.Api.People.Note;

namespace Sync
{
    public record NoteCategoryInfo
    {
        public string Channel { get; set; }
        public string Type { get; set; }
        public decimal Weight { get; set; }
        public string? Title { get; set; }
        public bool CopyContent { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
    }

    public interface IPostProcessConfig
    {
        void PostProcess();
    }
    
    public class NotesConfig : IPostProcessConfig
    {
        public List<NoteCategoryInfo> Categories { get; set; } = null!;
        public Dictionary<string,NoteCategoryInfo> CategoriesDict { get; set; } = null!;

        public void PostProcess()
        {
            CategoriesDict = Categories.SelectMany(nc => nc.Categories.Select(id => (key: id, value: nc)))
                .ToDictionary(p => p.key, p => p.value);
        }
    }

    public class NotesToActivitiesSync : Sync<Note>
    {
        private readonly PeopleClient _peopleClient;
        private readonly NotesConfig _config;

        public NotesToActivitiesSync(SyncDeps deps, PeopleClient peopleClient, NotesConfig config)
            : base(deps, peopleClient)
        {
            _peopleClient = peopleClient;
            _config = config;
        }

        public override async Task<DocumentRoot<List<Note>>> GetInitialDataAsync(string? nextUrl)
        {
            await base.GetInitialDataAsync(nextUrl);
            
            if (nextUrl != null)
            {
                return await _peopleClient.GetAsync<List<Note>>(nextUrl);
            }

            return await _peopleClient.GetAsync<List<Note>>("notes",
                ("order", "-created_at"));
        }

        public override async Task ProcessBatchAsync(Progress progress, Note note)
        {
            LastDate = note.CreatedAt;
            if (!_config.CategoriesDict.TryGetValue(note.NoteCategory.Id!, out var categoryInfo))
            {
                Log.Debug("Ignoring note with category {NoteCategoryId}", note.NoteCategory.Id);
                progress.Skipped++;
                return;
            }

            var noteCategory = await Deps.Cache.GetOrAddEntity(note.NoteCategory.Id!, async (categoryId) =>
            {
                var document = await _peopleClient.GetAsync<NoteCategory>($"note_categories/{categoryId}");
                return document.Data;
            });

            string? title = categoryInfo.Title;

            if (title == null)
            {
                var builder = new StringBuilder("A ")
                    .Append(noteCategory.Name);
                if (!noteCategory.Name.Contains("Note", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Append(" Note");
                }

                builder.Append(" was added by ");

                var noteTaker = await Deps.Cache.GetOrAddEntity(note.CreatedBy.Id!, async (personId) =>
                {
                    var document = await _peopleClient.GetAsync<Person>($"people/{personId}");
                    return document.Data;
                });

                builder.Append(noteTaker.FirstName);
                title = builder.ToString();
            }

            var activity = new UploadActivity(
                categoryInfo.Channel,
                categoryInfo.Type,
                OrbitUtil.ActivityKey(note),
                note.CreatedAt,
                categoryInfo.Weight,
                title,
                $"https://people.planningcenteronline.com/people/AC{note.Person.Id}/notes",
                "Note"
            );

            if (categoryInfo.CopyContent)
                activity.Description = note.Value;

            await UploadActivity(progress, note, activity, note.Person.Id!);
        }
    }
}