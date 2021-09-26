using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.People;

namespace Sync
{
    public static class Constants
    {
        public const string PlanningCenterSource = "planningcenter";
    }

    public class PeopleConfig
    {
        public string MetadataWorkspace { get; set; }
        public bool Initial { get; set; } = false;
    }
    
    public class PeopleToMembersSync : ISync<Person>
    {
        private readonly PeopleClient _peopleClient;
        private readonly OrbitApiClient _orbitClient;
        private SyncContext _context = null!;
        private readonly OrbitSync _orbitSync;
        private readonly PeopleConfig _config;
        private ApiCursor<Member> _memberCursor = null!;
        private readonly DataCache _cache;
        private readonly SyncImplConfig _syncConfig;

        public PeopleToMembersSync(PeopleClient peopleClient, OrbitApiClient orbitClient, OrbitSync orbitSync, 
            PeopleConfig config, DataCache cache, SyncImplConfig syncConfig)
        {
            _peopleClient = peopleClient;
            _orbitClient = orbitClient;
            _orbitSync = orbitSync;
            _config = config;
            _cache = cache;
            _syncConfig = syncConfig;
        }

        public string To => "Members";

        public Task<ApiCursor<Person>?> InitializeAsync(SyncContext context)
        {
            _context = context;
            string url;
            if (_config.Initial)
            {
                _memberCursor = new ApiCursor<Member>(
                    _orbitClient, UrlUtil.MakeUrl($"{_config.MetadataWorkspace}/members", 
                        ("sort", "name"),
                        ("direction", "ASC")));
                url = context.NextUrl ?? UrlUtil.MakeUrl("people",
                    ("order", "first_name"));
            }
            else
            {
                url = context.NextUrl ?? UrlUtil.MakeUrl("people",
                    ("order", "-created_at"));
            }
            
            
            return Task.FromResult(new ApiCursor<Person>(_peopleClient, url))!;
        }

        public async Task<SyncStatus> ProcessItemAsync(Person person)
        {
            if (!TryGetMetadata(person, out var memberMeta))
            {
                for (;;)
                {
                    if (!await _memberCursor.FetchNextAsync()) break;
                    foreach (var member in _memberCursor.Data!)
                    {
                        _cache.SetEntity(member);
                        var personId = member.Email.Split("@")[0];
                        _cache.SetMapping<Person>(personId, member.Id!);
                    }

                    if (TryGetMetadata(person, out memberMeta)) break;
                    var last = _memberCursor.Data.Last();
                    if (last.Name != null && person.FirstName[0] < last.Name[0]) break;
                }
            }
            
            List<string> tags = new();
            if (memberMeta != null && memberMeta.TagList.Any())
            {
                tags.AddRange(memberMeta.TagList);
            }
            
            var now = DateTime.Now.ToUniversalTime();
            try
            {
                var maybeCreated = await _orbitSync.CreateMemberAsync(person, tags);
                if (maybeCreated == null)
                {
                    return SyncStatus.Failed;
                }
                
                if (_syncConfig.KeyExistsMode == KeyExistsMode.Stop && maybeCreated.CreatedAt < now)
                {
                    _context.BatchProgress.Skipped++;
                    _context.BatchProgress.Complete = true;
                    return SyncStatus.Exists;
                }

                if (maybeCreated.Slug != person.Id)
                {
                    // for some reason, a second POST updates their slug
                    await _orbitSync.CreateMemberAsync(person, tags);
                }

                return SyncStatus.Success;
            }
            catch (ApiErrorException ex)
            {
                if (ex.Type == ErrorTypeEnum.AlreadyTaken)
                {
                    
                }
                return SyncStatus.Failed;
            }
        }

        private bool TryGetMetadata(Person person, out Member? member)
        {
            member = null;
            if (_cache.TryGetMapping<Person>(person.Id!, out var orbitId))
            {
                return _cache.TryGetEntity(orbitId!, out member);
            }

            return false;
        }
    }
}