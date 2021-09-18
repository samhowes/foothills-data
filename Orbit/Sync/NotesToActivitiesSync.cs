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
    public record NoteCategoryInfo(string Channel, string Type, decimal Weight, string? Title, bool CopyContent,
        params string[] Ids);

    public class NotesConfig
    {
        public Dictionary<string, NoteCategoryInfo> NoteCategoryInfos { get; } = new[]
            {
                new NoteCategoryInfo("Caring", "Caring Contact", 3m, null, false,
                    "230"), // pastoral notes => do not copy
                new NoteCategoryInfo("Caring", "Caring Contact", 3m, null, true, "225"),

                new NoteCategoryInfo("Caring", "Caring Kit", 4m, "A Caring Kit was delivered", true, "148408"),
                new NoteCategoryInfo("Caring", "Meal Train", 5m, "A meal train was set up", true, "9955"),

                new NoteCategoryInfo("Communication", "Information Update", 1m, null, true, "224", "51351"),
                new NoteCategoryInfo("Serving", "Serving Interest", 1m, null, true, "269", "44571"),
            }.SelectMany(nc => nc.Ids.Select(id => (key: id, value: nc)))
            .ToDictionary(p => p.key, p => p.value);
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
            return await _peopleClient.GetAsync<List<Note>>(nextUrl ?? "notes");
        }

        public override async Task ProcessBatchAsync(Progress progress, Note note)
        {
            LastDate = note.CreatedAt;
            if (!_config.NoteCategoryInfos.TryGetValue(note.NoteCategory.Id!, out var categoryInfo))
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