using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.DeleteChannel
{
    public class DeleteChannelHandler : IRequestHandler<DeleteChannelCommand, DeleteChannelResponse?>
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ILogger<DeleteChannelHandler> _logger;

        public DeleteChannelHandler(
            IChannelRepository channelRepository,
            ILogger<DeleteChannelHandler> logger)
        {
            _channelRepository = channelRepository;
            _logger = logger;
        }

        public async Task<DeleteChannelResponse?> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting channel. Id: {ChannelId}", request.Id);
                
                var deletedChannel = await _channelRepository.DeleteAsync(request.Id);

                if (deletedChannel == null)
                {
                    _logger.LogWarning("Channel not found for deletion. Id: {ChannelId}", request.Id);
                    return null;
                }

                _logger.LogInformation("Channel deleted successfully. Id: {ChannelId}, Name: {ChannelName}", 
                    deletedChannel.Id, deletedChannel.ChannelName);

                return new DeleteChannelResponse
                {
                    Id = deletedChannel.Id,
                    ChannelName = deletedChannel.ChannelName,
                    Category = deletedChannel.Category,
                    Subscribers = deletedChannel.Subscribers,
                    IsActive = deletedChannel.IsActive,
                    CreatedAt = deletedChannel.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting channel. Id: {ChannelId}", request.Id);
                throw;
            }
        }
    }
}


