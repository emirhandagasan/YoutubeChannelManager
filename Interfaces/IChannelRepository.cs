using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeChannelManager.DTOs;
using YoutubeChannelManager.Helpers;
using YoutubeChannelManager.Models;

namespace YoutubeChannelManager.Interfaces
{
    public interface IChannelRepository
    {
        Task<IEnumerable<Channel>> GetAllAsync(QueryObject query);
        Task<Channel?> GetByIdAsync(Guid id);
        Task<Channel> CreateAsync(Channel channelModel);
        Task<Channel?> UpdateAsync(Guid id, UpdateChannelRequestDto channelRequestDto);
        Task<Channel?> DeleteAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name);
    }
}