using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeChannelManager.BLL.DTOs.User
{
    public class ChangeRoleDto
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }
}