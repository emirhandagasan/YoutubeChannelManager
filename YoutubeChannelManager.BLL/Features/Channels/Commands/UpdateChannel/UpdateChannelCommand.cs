using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.UpdateChannel
{
    public record UpdateChannelCommand(Guid Id, string ChannelName, string Category, int Subscribers, bool IsActive)
        : IRequest<Channel?>;
}
