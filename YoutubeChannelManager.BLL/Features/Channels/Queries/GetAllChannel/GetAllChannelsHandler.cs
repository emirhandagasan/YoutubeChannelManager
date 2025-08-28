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
    public class GetAllChannelsHandler : IRequestHandler<GetAllChannelsQuery, IEnumerable<Channel>>
    {
        private readonly IChannelRepository _channelRepository;

        public GetAllChannelsHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<IEnumerable<Channel>> Handle(GetAllChannelsQuery request, CancellationToken cancellationToken)
        {
            return await _channelRepository.GetAllAsync(request.Query);
        }
    }
}

