using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.DeleteChannel
{
    public class DeleteChannelHandler : IRequestHandler<DeleteChannelCommand, Channel?>
    {
        private readonly IChannelRepository _channelRepository;

        public DeleteChannelHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<Channel?> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
        {
            return await _channelRepository.DeleteAsync(request.Id);
        }
    }
}
