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
        private readonly ILogger<CreateChannelHandler> _logger;

        public CreateChannelHandler(
            IChannelRepository channelRepository,
            ILogger<CreateChannelHandler> logger)
        {
            _channelRepository = channelRepository;
            _logger = logger;
        }

        public async Task<CreateChannelResponse> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Creating new channel. Name: {ChannelName}, Category: {Category}, Subscribers: {Subscribers}",
                    request.ChannelName,
                    request.Category,
                    request.Subscribers
                );

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

                _logger.LogInformation(
                    "Channel created successfully. Id: {ChannelId}, Name: {ChannelName}",
                    channel.Id,
                    channel.ChannelName
                );

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
                _logger.LogError(
                    ex,
                    "Error creating channel. Name: {ChannelName}",
                    request.ChannelName
                );
                throw;
            }
        }
    }
}
