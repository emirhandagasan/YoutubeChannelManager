using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeChannelManager.Models
{
    public class Channel
    {
        public Guid Id { get; set; }
        public string ChannelName { get; set; }
        public string Category { get; set; }
        public int Subscribers { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}