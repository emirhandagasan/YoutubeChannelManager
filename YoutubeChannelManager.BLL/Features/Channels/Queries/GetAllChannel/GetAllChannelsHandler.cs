using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Queries.GetAllChannel
{
    public class GetAllChannelsHandler : IRequestHandler<GetAllChannelsQuery, IEnumerable<GetAllChannelsResponse>>
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ILogger<GetAllChannelsHandler> _logger;

        public GetAllChannelsHandler(
            IChannelRepository channelRepository,
            ILogger<GetAllChannelsHandler> logger)
        {
            _channelRepository = channelRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<GetAllChannelsResponse>> Handle(GetAllChannelsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all channels with query: {@Query}", request.Query);

                var channels = await _channelRepository.GetAllAsync(request.Query);

                var response = channels.Select(c => new GetAllChannelsResponse
                {
                    Id = c.Id,
                    ChannelName = c.ChannelName,
                    Category = c.Category,
                    Subscribers = c.Subscribers,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} channels", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all channels");
                throw;
            }
        }
    }
}


