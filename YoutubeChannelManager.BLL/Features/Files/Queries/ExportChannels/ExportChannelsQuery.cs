using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Helpers;

namespace YoutubeChannelManager.BLL.Features.Files.Queries.ExportChannels
{
    public class ExportChannelsQuery : IRequest<ExportChannelsResponse>
    {
        public QueryObject Query { get; set; }
        public string Format { get; set; } = "csv";

        public ExportChannelsQuery(QueryObject query, string format = "csv")
        {
            Query = query;
            Format = format;
        }
    }
}
