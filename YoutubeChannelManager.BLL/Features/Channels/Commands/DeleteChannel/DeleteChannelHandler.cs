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
    public class DeleteChannelHandler : IRequestHandler<DeleteChannelCommand, DeleteChannelResponse?>
    {
        private readonly IChannelRepository _channelRepository;

        public DeleteChannelHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<DeleteChannelResponse?> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
        {
            var deletedChannel = await _channelRepository.DeleteAsync(request.Id);

            if (deletedChannel == null)
                return null;

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
    }
}

