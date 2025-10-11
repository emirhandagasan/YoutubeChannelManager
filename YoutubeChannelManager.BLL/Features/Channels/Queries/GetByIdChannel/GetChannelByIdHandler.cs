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
        private readonly ILogger<GetChannelByIdHandler> _logger;

        public GetChannelByIdHandler(
            IChannelRepository channelRepository,
            ILogger<GetChannelByIdHandler> logger)
        {
            _channelRepository = channelRepository;
            _logger = logger;
        }

        public async Task<GetChannelByIdResponse?> Handle(GetChannelByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching channel by Id: {ChannelId}", request.Id);

                var channel = await _channelRepository.GetByIdAsync(request.Id);

                if (channel == null)
                {
                    _logger.LogWarning("Channel not found. Id: {ChannelId}", request.Id);
                    return null;
                }

                _logger.LogInformation("Channel retrieved successfully. Id: {ChannelId}, Name: {ChannelName}", 
                    channel.Id, channel.ChannelName);

                return new GetChannelByIdResponse
                {
                    Id = channel.Id,
                    ChannelName = channel.ChannelName,
                    Category = channel.Category,
                    Subscribers = channel.Subscribers,
                    IsActive = channel.IsActive,
                    CreatedAt = channel.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching channel by Id: {ChannelId}", request.Id);
                throw;
            }
        }
    }
}

