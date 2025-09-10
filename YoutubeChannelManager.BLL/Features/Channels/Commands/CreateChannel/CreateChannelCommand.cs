using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.CreateChannel
{
    public class CreateChannelCommand : IRequest<CreateChannelResponse>
    {
        public string ChannelName { get; }
        public string Category { get; }
        public int Subscribers { get; }
        public bool IsActive { get; }

        public CreateChannelCommand(string channelName, string category, int subscribers, bool isActive)
        {
            ChannelName = channelName;
            Category = category;
            Subscribers = subscribers;
            IsActive = isActive;
        }
    }
}
