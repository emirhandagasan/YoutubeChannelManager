using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Service
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }


        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(key);
                
                if (string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogDebug("Cache miss: {Key}", key);
                    return null;
                }

                _logger.LogDebug("Cache hit: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting from cache: {Key}", key);
                return null;
            }
        }


        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
                };

                await _cache.SetStringAsync(key, serialized, options);
                _logger.LogDebug("Cached: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Cache removed: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache: {Key}", key);
            }
        }
    }
}
