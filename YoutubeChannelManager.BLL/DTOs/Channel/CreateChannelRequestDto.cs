using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeChannelManager.BLL.DTOs
{
    public class CreateChannelRequestDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "Channel name must be at least 2 characters long.")]
        [MaxLength(80, ErrorMessage = "Channel name cannot exceed 80 characters.")]
        public string ChannelName { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Category must be at least 5 characters long.")]
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Subscribers must be a non-negative integer and cannot be higher than 2,147,483,647")]
        public int Subscribers { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
