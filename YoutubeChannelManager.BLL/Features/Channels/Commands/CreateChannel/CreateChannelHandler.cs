using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.CreateChannel
{
    public class CreateChannelHandler : IRequestHandler<CreateChannelCommand, Channel>
    {
        private readonly IChannelRepository _channelRepository;

        public CreateChannelHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<Channel> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            var channel = new Channel
            {
                ChannelName = request.ChannelName,
                Category = request.Category,
                Subscribers = request.Subscribers,
                IsActive = request.IsActive
            };

            return await _channelRepository.CreateAsync(channel);
        }
    }
}
