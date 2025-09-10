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
     public class CreateChannelHandler : IRequestHandler<CreateChannelCommand, CreateChannelResponse>
    {
        private readonly IChannelRepository _channelRepository;

        public CreateChannelHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<CreateChannelResponse> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                ChannelName = request.ChannelName,
                Category = request.Category,
                Subscribers = request.Subscribers,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _channelRepository.CreateAsync(channel);

            return new CreateChannelResponse
            {
                Id = channel.Id,
                ChannelName = channel.ChannelName,
                Category = channel.Category,
                Subscribers = channel.Subscribers,
                IsActive = channel.IsActive,
            };
        }
    }
}
