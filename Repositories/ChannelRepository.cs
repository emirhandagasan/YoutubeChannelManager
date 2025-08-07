using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YoutubeChannelManager.Data;
using YoutubeChannelManager.DTOs;
using YoutubeChannelManager.Interfaces;
using YoutubeChannelManager.Models;

namespace YoutubeChannelManager.Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly ApplicationDbContext _db;

        public ChannelRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IEnumerable<Channel>> GetAllAsync(string? search, string? category, string? sort)
        {
            var query = _db.Channels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.ChannelName.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(c => c.Category == category);

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort switch
                {
                    "subscribers_asc" => query.OrderBy(c => c.Subscribers),
                    "subscribers_desc" => query.OrderByDescending(c => c.Subscribers),
                    _ => query
                };
            }

            return await query.ToListAsync();
        }


        public async Task<Channel?> GetByIdAsync(Guid id)
        {
            return await _db.Channels.FindAsync(id);
        }


        public async Task<Channel> CreateAsync(Channel channelModel)
        {
            await _db.Channels.AddAsync(channelModel);
            await _db.SaveChangesAsync();
            return channelModel;
        }


        public async Task<Channel?> UpdateAsync(Guid id, UpdateChannelRequestDto channelRequestDto)
        {
            var existingChannel = await _db.Channels.FirstOrDefaultAsync(c => c.Id == id);

            if (existingChannel == null)
            {
                return null;
            }

            existingChannel.ChannelName = channelRequestDto.ChannelName;
            existingChannel.Category = channelRequestDto.Category;
            existingChannel.Subscribers = channelRequestDto.Subscribers;
            existingChannel.IsActive = channelRequestDto.IsActive;

            await _db.SaveChangesAsync();

            return existingChannel;
        }


        public async Task<Channel?> DeleteAsync(Guid id)
        {
            var channelModel = await _db.Channels.FirstOrDefaultAsync(x => x.Id == id);

            if (channelModel == null)
            {
                return null;    
            }

            _db.Channels.Remove(channelModel);
            await _db.SaveChangesAsync();
            return channelModel;
        }
    }
}