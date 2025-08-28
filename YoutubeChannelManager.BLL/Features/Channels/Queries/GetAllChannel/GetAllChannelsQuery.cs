using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Helpers;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Queries.GetAllChannel
{
    public record GetAllChannelsQuery(QueryObject Query) : IRequest<IEnumerable<Channel>>;
}
