using MediatR;
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

        public GetChannelByIdHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<GetChannelByIdResponse?> Handle(GetChannelByIdQuery request, CancellationToken cancellationToken)
        {
            var channel = await _channelRepository.GetByIdAsync(request.Id);

            if (channel == null)
                return null;

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
    }
}
