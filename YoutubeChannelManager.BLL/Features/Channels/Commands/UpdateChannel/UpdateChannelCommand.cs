using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.DTOs;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.UpdateChannel
{
    public class UpdateChannelCommand : IRequest<UpdateChannelResponse?>
    {
        public Guid Id { get; }
        public string ChannelName { get; }
        public string Category { get; }
        public int Subscribers { get; }
        public bool IsActive { get; }

        public UpdateChannelCommand(Guid id, string channelName, string category, int subscribers, bool isActive)
        {
            Id = id;
            ChannelName = channelName;
            Category = category;
            Subscribers = subscribers;
            IsActive = isActive;
        }
    }
}

