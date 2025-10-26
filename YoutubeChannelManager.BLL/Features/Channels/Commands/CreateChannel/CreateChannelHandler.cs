using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.CreateChannel
{
    public class CreateChannelHandler : IRequestHandler<CreateChannelCommand, CreateChannelResponse>
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CreateChannelHandler> _logger;

        public CreateChannelHandler(
            IChannelRepository channelRepository,
            ICacheService cacheService,
            ILogger<CreateChannelHandler> logger)
        {
            _channelRepository = channelRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<CreateChannelResponse> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating channel. Name: {ChannelName}", request.ChannelName);

                var channel = new Channel
                {
                    Id = Guid.NewGuid(),
                    ChannelName = request.ChannelName,
                    Category = request.Category,
                    Subscribers = request.Subscribers,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                await _channelRepository.CreateAsync(channel);


                await _cacheService.RemoveAsync("channels:all");

                _logger.LogInformation("Channel created. Id: {ChannelId}", channel.Id);

                return new CreateChannelResponse
                {
                    Id = channel.Id,
                    ChannelName = channel.ChannelName,
                    Category = channel.Category,
                    Subscribers = channel.Subscribers,
                    IsActive = channel.IsActive,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating channel");
                throw;
            }
        }
    }
}
