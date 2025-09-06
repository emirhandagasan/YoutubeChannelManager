using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.DTOs;
using YoutubeChannelManager.BLL.Helpers;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Data;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly ApplicationDbContext _db;

        public ChannelRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IEnumerable<Channel>> GetAllAsync(QueryObject query)
        {
            var stocks = _db.Channels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.ChannelName))
            {
                stocks = stocks.Where(c => c.ChannelName.Contains(query.ChannelName));
            }

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                stocks = stocks.Where(c => c.Category.Contains(query.Category));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("ChannelName", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDecsending ? stocks.OrderByDescending(c => c.ChannelName) : stocks.OrderBy(c => c.ChannelName);
                }
                else if (query.SortBy.Equals("Subscribers", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDecsending ? stocks.OrderByDescending(c => c.Subscribers) : stocks.OrderBy(c => c.Subscribers);
                }
                else if (query.SortBy.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDecsending ? stocks.OrderByDescending(c => c.Category) : stocks.OrderBy(c => c.Category);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
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


        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Channels
                .AnyAsync(c => c.ChannelName.ToLower() == name.ToLower());
        }
    }
}
