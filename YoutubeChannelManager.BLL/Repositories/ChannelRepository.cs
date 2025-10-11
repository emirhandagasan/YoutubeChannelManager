using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ChannelRepository> _logger;

        public ChannelRepository(ApplicationDbContext db, ILogger<ChannelRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Channel>> GetAllAsync(QueryObject query)
        {
            try
            {
                _logger.LogDebug("Fetching channels with query: {@Query}", query);

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
                var result = await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();

                _logger.LogInformation("Retrieved {Count} channels", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching channels");
                throw;
            }
        }

        public async Task<Channel?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Fetching channel by Id: {ChannelId}", id);

                var channel = await _db.Channels.FindAsync(id);

                if (channel == null)
                {
                    _logger.LogWarning("Channel not found. Id: {ChannelId}", id);
                }
                else
                {
                    _logger.LogDebug("Channel found. Id: {ChannelId}, Name: {ChannelName}", id, channel.ChannelName);
                }

                return channel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching channel by Id: {ChannelId}", id);
                throw;
            }
        }

        public async Task<Channel> CreateAsync(Channel channelModel)
        {
            try
            {
                _logger.LogInformation("Creating channel: {ChannelName}", channelModel.ChannelName);

                await _db.Channels.AddAsync(channelModel);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Channel created successfully. Id: {ChannelId}", channelModel.Id);
                return channelModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating channel: {ChannelName}", channelModel.ChannelName);
                throw;
            }
        }

        public async Task<Channel?> UpdateAsync(Guid id, UpdateChannelRequestDto channelRequestDto)
        {
            try
            {
                _logger.LogInformation("Updating channel. Id: {ChannelId}", id);

                var existingChannel = await _db.Channels.FirstOrDefaultAsync(c => c.Id == id);

                if (existingChannel == null)
                {
                    _logger.LogWarning("Channel not found for update. Id: {ChannelId}", id);
                    return null;
                }

                existingChannel.ChannelName = channelRequestDto.ChannelName;
                existingChannel.Category = channelRequestDto.Category;
                existingChannel.Subscribers = channelRequestDto.Subscribers;
                existingChannel.IsActive = channelRequestDto.IsActive;

                await _db.SaveChangesAsync();

                _logger.LogInformation("Channel updated successfully. Id: {ChannelId}, Name: {ChannelName}", id, existingChannel.ChannelName);
                return existingChannel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating channel. Id: {ChannelId}", id);
                throw;
            }
        }

        public async Task<Channel?> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting channel. Id: {ChannelId}", id);

                var channelModel = await _db.Channels.FirstOrDefaultAsync(x => x.Id == id);

                if (channelModel == null)
                {
                    _logger.LogWarning("Channel not found for deletion. Id: {ChannelId}", id);
                    return null;
                }

                _db.Channels.Remove(channelModel);
                
                await _db.SaveChangesAsync();

                _logger.LogInformation("Channel deleted successfully. Id: {ChannelId}, Name: {ChannelName}", id, channelModel.ChannelName);
                return channelModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting channel. Id: {ChannelId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            try
            {
                _logger.LogDebug("Checking if channel exists by name: {ChannelName}", name);

                var exists = await _db.Channels.AnyAsync(c => EF.Functions.ILike(c.ChannelName, name));
                
                _logger.LogDebug("Channel exists: {Exists}, Name: {ChannelName}", exists, name);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking channel existence by name: {ChannelName}", name);
                throw;
            }
        }
    }
}
