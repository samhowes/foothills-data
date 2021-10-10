using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using Microsoft.Extensions.Options;
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
            PeopleConfig config, DataCache cache, IOptions<SyncImplConfig> syncConfig)
        {
            _peopleClient = peopleClient;
            _orbitClient = orbitClient;
            _orbitSync = orbitSync;
            _config = config;
            _cache = cache;
            _syncConfig = syncConfig.Value;
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
            var memberMeta = await GetMetadata(person);
            
            List<string> tags = new();
            if (memberMeta != null && memberMeta.TagList.Any())
            {
                tags.AddRange(memberMeta.TagList);
            }
            
            var now = DateTime.Now.ToUniversalTime();
            Member? maybeCreated = null;
            try
            {
                maybeCreated = await _orbitSync.CreateMemberAsync(person, tags);
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
            }
            catch (ApiErrorException ex)
            {
                if (ex.Type != ErrorTypeEnum.AlreadyTaken)
                {
                    return SyncStatus.Failed;
                }

                var doc = await _orbitClient.GetAsync<Member>(
                    UrlUtil.MakeUrl($"{person.OrbitWorkspace}/members/find",
                        ("uid", person.Id),
                        ("source", Constants.PlanningCenterSource)));
                if (doc == null) return SyncStatus.Failed;
                await _orbitSync.UpdateMemberAsync(doc.Data.Id!, person, tags);

                try
                {
                    await _orbitClient.PostAsync<OtherIdentity>(
                        $"{person.OrbitWorkspace}/members/{doc.Data.Id}/identities",
                        new OtherIdentity("email")
                        {
                            Email = $"{person.Id}@foothillsuu.org"
                        });
                }
                catch
                {
                    // ignored
                }
                
                return SyncStatus.Success;
            }

            if (maybeCreated.Slug != person.Id)
            {
                // for some reason, a second POST updates their slug
                await _orbitSync.CreateMemberAsync(person, tags);
                
            }
            return SyncStatus.Success;
        }

        private async Task<Member?> GetMetadata(Person person)
        {
            if (TryGetMetadata(person, out var memberMeta)) return memberMeta;
            if (_config.Initial)
            {
                for (;;)
                {
                    if (!await _memberCursor.FetchNextAsync()) break;
                    foreach (var member in _memberCursor.Data!)
                    {
                        SetMetadata(member);
                    }

                    if (TryGetMetadata(person, out memberMeta)) break;
                    var last = _memberCursor.Data.Last();
                    if (last.Name != null && person.FirstName[0] < last.Name[0]) break;
                }    
            }
            else
            {
                try
                {
                    var doc = await _orbitClient.GetAsync<Member>(
                        UrlUtil.MakeUrl($"{_config.MetadataWorkspace}/members/find",
                            ("email", $"{person.Id}@foothillsuu.org"),
                            ("source", "email")));
                    memberMeta = doc.Data;
                    if (memberMeta != null)
                    {
                        SetMetadata(memberMeta);    
                    }
                }
                catch (ApiErrorException ex)
                {
                    if (ex.Type != ErrorTypeEnum.NotFound) throw;
                }
            }
            
            return memberMeta;
        }

        private void SetMetadata(Member member)
        {
            _cache.SetEntity(member);
            var personId = member.Email.Split("@")[0];
            _cache.SetMapping<Person>(personId, member.Id!);
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