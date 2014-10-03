using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JimBobBennett.JimLib.Extensions;
using Newtonsoft.Json;

namespace JimBobBennett.JimLib.Collections
{
    public class CacheWithFail<TKey, T>
        where T : class
    {
// ReSharper disable once StaticFieldInGenericType
        private readonly TimeSpan _failedRetryWaitTime = TimeSpan.FromMinutes(10);

        private readonly object _failedSyncObj = new object();
        private readonly object _cacheSyncObj = new object();

        private readonly Dictionary<TKey, T> _cache = new Dictionary<TKey, T>();
        private readonly Dictionary<TKey, DateTime> _failedCache = new Dictionary<TKey, DateTime>();

        public CacheWithFail(TimeSpan failedRetryTime)
        {
            _failedRetryWaitTime = failedRetryTime;
        }

        public CacheWithFail()
        {
        }

        public CacheState TryGetValue(TKey id, out T result)
        {
            lock (_failedSyncObj)
            {
                DateTime failedDate;
                if (_failedCache.TryGetValue(id, out failedDate))
                {
                    if (failedDate < DateTime.Now)
                        _failedCache.Remove(id);
                    else
                    {
                        result = null;
                        return CacheState.Failed;
                    }
                }
            }

            lock (_cacheSyncObj)
                return _cache.TryGetValue(id, out result) ? CacheState.Found : CacheState.NotFound;
        }

        public void Add(TKey id, T value)
        {
            lock (_failedSyncObj)
                _failedCache.Remove(id);

            lock (_cacheSyncObj)
                _cache[id] = value;
        }

        public bool Remove(TKey id)
        {
            lock (_failedSyncObj)
                _failedCache.Remove(id);

            lock (_cacheSyncObj)
                return _cache.Remove(id);
        }

        public void MarkAsFailed(TKey id)
        {
            lock (_cacheSyncObj)
                _cache.Remove(id);

            lock (_failedSyncObj)
                _failedCache[id] = DateTime.Now.Add(_failedRetryWaitTime);
        }

        public async Task<T> GetOrAddAsync(TKey id, Func<TKey, Task<T>> loadAction)
        {
            T result;
            var cacheResult = TryGetValue(id, out result);

            if (cacheResult == CacheState.Found)
                return result;
            if (cacheResult == CacheState.Failed)
                return null;

            result = await loadAction(id);

            if (result == null)
            {
                MarkAsFailed(id);
                return null;
            }

            Add(id, result);

            return result;
        }

        public string DumpCacheAsJson()
        {
            lock (_cacheSyncObj)
                return JsonConvert.SerializeObject(_cache);
        }

        public void LoadCacheFromJson(string json)
        {
            if (json.IsNullOrEmpty()) return;

            lock (_cacheSyncObj)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<TKey, T>>(json);
                    foreach (var kvp in data)
                        _cache[kvp.Key] = kvp.Value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to deserialize cache: " + ex.Message);
                }
            }
        }
    }
}
