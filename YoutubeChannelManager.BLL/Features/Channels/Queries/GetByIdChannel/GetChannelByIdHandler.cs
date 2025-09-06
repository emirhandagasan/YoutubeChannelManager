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
    public class GetChannelByIdHandler : IRequestHandler<GetChannelByIdQuery, Channel?>
    {
        private readonly IChannelRepository _channelRepository;

        public GetChannelByIdHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<Channel?> Handle(GetChannelByIdQuery request, CancellationToken cancellationToken)
        {
            return await _channelRepository.GetByIdAsync(request.Id);
        }
    }
}
