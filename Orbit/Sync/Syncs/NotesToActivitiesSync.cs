using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApi;
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

    public class NotesToActivitiesSync : ISync<Note>
    {
        private readonly SyncDeps _deps;
        private readonly PeopleClient _peopleClient;
        private readonly NotesConfig _config;
        private readonly OrbitSync _orbitSync;
        private SyncContext _context = null!;

        public NotesToActivitiesSync(SyncDeps deps, PeopleClient peopleClient, NotesConfig config, OrbitSync orbitSync)
        {
            _deps = deps;
            _peopleClient = peopleClient;
            _config = config;
            _orbitSync = orbitSync;
        }

        public Task<PlanningCenterCursor<Note>?> InitializeAsync(SyncContext context)
        {
            _context = context;
            var url = context.NextUrl ?? UrlUtil.MakeUrl("notes",
                ("order", "-created_at"));
            return Task.FromResult(new PlanningCenterCursor<Note>(_peopleClient, url))!;
        }

        public async Task ProcessItemAsync(Note note)
        {
            var progress = _context.BatchProgress;
            if (!_config.CategoriesDict.TryGetValue(note.NoteCategory.Id!, out var categoryInfo))
            {
                _deps.Log.Debug("Ignoring note with category {NoteCategoryId}", note.NoteCategory.Id);
                progress.Skipped++;
                return;
            }

            var noteCategory = await _deps.Cache.GetOrAddEntity(note.NoteCategory.Id!, async (categoryId) =>
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

                var noteTaker = await _deps.Cache.GetOrAddEntity(note.CreatedBy.Id!, async (personId) =>
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

            await _orbitSync.UploadActivity<Note,Note>(progress, note, activity, note.Person.Id!);
        }
    }
}