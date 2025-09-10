using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Commands.DeleteChannel
{
    public class DeleteChannelCommand : IRequest<DeleteChannelResponse?>
    {
        public Guid Id { get; }

        public DeleteChannelCommand(Guid id)
        {
            Id = id;
        }
    }
}
