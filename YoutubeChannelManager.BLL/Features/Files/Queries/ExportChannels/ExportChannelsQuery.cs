using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Helpers;

namespace YoutubeChannelManager.BLL.Features.Files.Queries.ExportChannels
{
    public record ExportChannelsQuery(QueryObject Query, string Format) : IRequest<byte[]>;
}
