using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.People;
using Note = PlanningCenter.Api.People.Note;

namespace Sync
{
    public record NoteCategoryInfo(string Channel, string Type, decimal Weight, bool CopyContent, params string[] Ids);
    public class NotesConfig
    {
        public Dictionary<string, NoteCategoryInfo> NoteCategoryInfos { get; } = new[]
            {
                new NoteCategoryInfo("Caring", "Caring Contact", 3m, false, "230"), // pastoral notes => do not copy
                new NoteCategoryInfo("Caring", "Caring Contact", 3m, true, "225", "9955", "148408"),
                new NoteCategoryInfo("Communication", "Information Update", 1m, true, "224", "51351"),
                new NoteCategoryInfo("Serving", "Serving Interest", 1m, true, "269", "44571"),
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

            var activity = new UploadActivity(
                categoryInfo.Channel, 
                categoryInfo.Type, 
                OrbitUtil.ActivityKey(note), 
                note.CreatedAt, 
                categoryInfo.Weight, 
                $"A {noteCategory.Name} Note was added", 
                note.Links.Self(), // todo get app template link 
                "Note"
            );

            if (categoryInfo.CopyContent)
                activity.Description = note.Value;
            
            await UploadActivity(progress, note, activity, note.Person.Id!);
        }
    }
}