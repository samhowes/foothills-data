using System.Threading.Tasks;
using JsonApi;

namespace Sync
{
    public class CheckInsToActivitiesSync : ISync
    {
        public CheckInsToActivitiesSync()
        {
            
        }

        public string From => "CheckIns";
        public string To => "Activities";
        public Task<Response> GetInitialDataAsync()
        {
            _checkIns = await 
        }

        public Task ProcessBatchAsync(Stats stats)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> GetNextBatchAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}