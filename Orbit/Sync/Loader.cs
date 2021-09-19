using System;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;

namespace Sync
{
    public class Loader<T> where T : EntityBase
    {
        private readonly ApiCursor<T> _cursor;
        private readonly Func<T, DateTime> _getDate;
        private readonly OrbitInfo _info;
        private DataCache _cache;

        public Loader(ApiCursor<T> cursor, Func<T,DateTime> getDate, DataCache cache)
        {
            _cursor = cursor;
            _getDate = getDate;
            _cache = cache;
            _info = new OrbitInfo();
        }
        
        public async Task<OrbitInfo> GetUntil(DateTime date)
        {
            string? url = null!;
            int ShouldContinue()
            {
                int direction = 0;
                if (date < _info.MinDate)
                {
                    if (_info.NextUrl == null) return 0;
                    url = _info.NextUrl;
                    direction = 1;
                }
                
                if (date > _info.MaxDate)
                {
                    if (_info.PrevUrl == null) return 0;
                    url = _info.PrevUrl;
                    direction = -1;
                }
                
                return direction;    
            }

            var direction = ShouldContinue();
            if (direction <= 0) return _info;
            for (;;)
            {
                await LoadBatch();
                
                direction = ShouldContinue();
                if (direction <= 0) break;
            }

            return _info;
        }

        private async Task LoadBatch()
        {
            if (!await _cursor.FetchNextAsync()) return;
            foreach (var item in _cursor.Data!)
            {
                _cache.SetEntity(item);
            }
            
            // assume a descending order cursor
            _info.MaxDate = MaxDate(_getDate(_cursor.Data.First()), _info.MaxDate);
            _info.MinDate = MinDate(_getDate(_cursor.Data.Last()), _info.MinDate);
        }

        private DateTime MinDate(DateTime a, DateTime b) => a < b ? a : b;

        private DateTime MaxDate(DateTime a, DateTime b) => a > b ? a : b;
    }
}