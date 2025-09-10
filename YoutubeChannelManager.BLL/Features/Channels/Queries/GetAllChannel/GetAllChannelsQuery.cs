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
    public class GetAllChannelsQuery : IRequest<IEnumerable<GetAllChannelsResponse>>
    {
        public QueryObject Query { get; }

        public GetAllChannelsQuery(QueryObject query)
        {
            Query = query;
        }
    }
}
