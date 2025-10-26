using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeChannelManager.BLL.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
    }
}