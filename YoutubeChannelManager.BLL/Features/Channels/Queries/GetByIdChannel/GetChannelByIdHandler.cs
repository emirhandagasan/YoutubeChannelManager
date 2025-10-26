using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Queries.GetByIdChannel
{
    public class GetChannelByIdHandler : IRequestHandler<GetChannelByIdQuery, GetChannelByIdResponse?>
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetChannelByIdHandler> _logger;

        public GetChannelByIdHandler(
            IChannelRepository channelRepository,
            ICacheService cacheService,
            ILogger<GetChannelByIdHandler> logger)
        {
            _channelRepository = channelRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<GetChannelByIdResponse?> Handle(GetChannelByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"channel:{request.Id}";


                var cachedChannel = await _cacheService.GetAsync<GetChannelByIdResponse>(cacheKey);
                if (cachedChannel != null)
                {
                    _logger.LogInformation("Channel from cache. Id: {ChannelId}", request.Id);
                    return cachedChannel;
                }


                _logger.LogInformation("Fetching channel from database. Id: {ChannelId}", request.Id);
                var channel = await _channelRepository.GetByIdAsync(request.Id);

                if (channel == null)
                {
                    _logger.LogWarning("Channel not found. Id: {ChannelId}", request.Id);
                    return null;
                }


                var response = new GetChannelByIdResponse
                {
                    Id = channel.Id,
                    ChannelName = channel.ChannelName,
                    Category = channel.Category,
                    Subscribers = channel.Subscribers,
                    IsActive = channel.IsActive,
                    CreatedAt = channel.CreatedAt
                };


                await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

                _logger.LogInformation("Channel fetched and cached. Id: {ChannelId}", request.Id);
                return response;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching channel. Id: {ChannelId}", request.Id);
                throw;
            }
        }
    }
}

