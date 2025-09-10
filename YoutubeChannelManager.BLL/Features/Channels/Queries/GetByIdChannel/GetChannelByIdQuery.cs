using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Features.Channels.Queries.GetByIdChannel
{
    public class GetChannelByIdQuery : IRequest<GetChannelByIdResponse?>
    {
        public Guid Id { get; }

        public GetChannelByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
