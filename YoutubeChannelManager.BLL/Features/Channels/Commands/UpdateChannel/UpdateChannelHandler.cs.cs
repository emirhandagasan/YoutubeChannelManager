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
    public class UpdateChannelHandler : IRequestHandler<UpdateChannelCommand, UpdateChannelResponse>
    {
        private readonly IChannelRepository _channelRepository;

        public UpdateChannelHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<UpdateChannelResponse> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
        {
            var updateDto = new UpdateChannelRequestDto
            {
                ChannelName = request.ChannelName,
                Category = request.Category,
                Subscribers = request.Subscribers,
                IsActive = request.IsActive
            };

            var updatedChannel = await _channelRepository.UpdateAsync(request.Id, updateDto);

            if (updatedChannel == null)
            {
                return null;
            }

            return new UpdateChannelResponse
            {
                Id = updatedChannel.Id,
                ChannelName = updatedChannel.ChannelName,
                Category = updatedChannel.Category,
                Subscribers = updatedChannel.Subscribers,
                IsActive = updatedChannel.IsActive,
            };
        }
    }
}



