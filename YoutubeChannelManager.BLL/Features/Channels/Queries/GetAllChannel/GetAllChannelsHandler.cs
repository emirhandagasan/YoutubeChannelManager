using MediatR;
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

        public GetAllChannelsHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<IEnumerable<GetAllChannelsResponse>> Handle(GetAllChannelsQuery request, CancellationToken cancellationToken)
        {
            var channels = await _channelRepository.GetAllAsync(request.Query);

            return channels.Select(c => new GetAllChannelsResponse
            {
                Id = c.Id,
                ChannelName = c.ChannelName,
                Category = c.Category,
                Subscribers = c.Subscribers,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            });
        }
    }
}

