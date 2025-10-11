using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.DTOs;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.UpdateChannel
{
    public class UpdateChannelHandler : IRequestHandler<UpdateChannelCommand, UpdateChannelResponse>
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ILogger<UpdateChannelHandler> _logger;

        public UpdateChannelHandler(
            IChannelRepository channelRepository,
            ILogger<UpdateChannelHandler> logger)
        {
            _channelRepository = channelRepository;
            _logger = logger;
        }

        public async Task<UpdateChannelResponse> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating channel. Id: {ChannelId}, Name: {ChannelName}", 
                    request.Id, request.ChannelName);

                var updateDto = new UpdateChannelRequestDto
                {
                    ChannelName = request.ChannelName,
                    Category = request.Category,
                    Subscribers = request.Subscribers,
                    IsActive = request.IsActive
                };

                var updatedChannel = await _channelRepository.UpdateAsync(request.Id, updateDto);

                if (updatedChannel == null)
                {
                    _logger.LogWarning("Channel not found for update. Id: {ChannelId}", request.Id);
                    return null;
                }

                _logger.LogInformation("Channel updated successfully. Id: {ChannelId}, Name: {ChannelName}", 
                    updatedChannel.Id, updatedChannel.ChannelName);

                return new UpdateChannelResponse
                {
                    Id = updatedChannel.Id,
                    ChannelName = updatedChannel.ChannelName,
                    Category = updatedChannel.Category,
                    Subscribers = updatedChannel.Subscribers,
                    IsActive = updatedChannel.IsActive,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating channel. Id: {ChannelId}", request.Id);
                throw;
            }
        }
    }
}




