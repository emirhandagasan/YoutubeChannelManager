using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeChannelManager.BLL.Features.Channels.Queries.GetByIdChannel
{
    public class GetChannelByIdResponse
    {
        public Guid Id { get; set; }
        public string ChannelName { get; set; }
        public string Category { get; set; }
        public int Subscribers { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}