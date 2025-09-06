using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.DTOs;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.UpdateChannel
{
    public class UpdateChannelHandler : IRequestHandler<UpdateChannelCommand, Channel?>
    {
        private readonly IChannelRepository _channelRepository;

        public UpdateChannelHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<Channel?> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
        {
            var dto = new UpdateChannelRequestDto
            {
                ChannelName = request.ChannelName,
                Category = request.Category,
                Subscribers = request.Subscribers,
                IsActive = request.IsActive
            };

            return await _channelRepository.UpdateAsync(request.Id, dto);
        }
    }
}
