using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YoutubeChannelManager.DTOs;
using YoutubeChannelManager.Interfaces;
using YoutubeChannelManager.Models;

namespace YoutubeChannelManager.Controllers
{

    [ApiController]
    [Route("api/channels")]
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IFileService _fileService;

        public ChannelsController(IChannelRepository channelRepository, IFileService fileService)
        {
            _channelRepository = channelRepository;
            _fileService = fileService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? category, [FromQuery] string? sort)
        {
            var result = await _channelRepository.GetAllAsync(search, category, sort);
            return Ok(result);
        }


        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var channel = await _channelRepository.GetByIdAsync(id);

            if (channel == null)
            {
                return NotFound();
            }

            return Ok(channel);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChannelRequestDto channelDto)
        {
            var channelModel = new Channel
            {
                ChannelName = channelDto.ChannelName,
                Category = channelDto.Category,
                Subscribers = channelDto.Subscribers,
                IsActive = channelDto.IsActive
            };

            await _channelRepository.CreateAsync(channelModel);


            return CreatedAtAction(nameof(GetById), new { id = channelModel.Id }, new CreateChannelRequestDto
            {
                ChannelName = channelModel.ChannelName,
                Category = channelModel.Category,
                Subscribers = channelModel.Subscribers,
                IsActive = channelModel.IsActive
            });
        }


        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateChannelRequestDto updateChannelRequestDto)
        {
            var channelModel = await _channelRepository.UpdateAsync(id, updateChannelRequestDto);

            if (channelModel == null)
            {
                return NotFound();
            }


            return Ok(new UpdateChannelRequestDto
            {
                ChannelName = channelModel.ChannelName,
                Category = channelModel.Category,
                Subscribers = channelModel.Subscribers,
                IsActive = channelModel.IsActive
            });
        }


        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var stockModel = await _channelRepository.DeleteAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpPost("import/csv")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .csv files are supported.");

            await _fileService.ImportCsvAsync(file.OpenReadStream());
            return Ok("CSV File imported successfully.");
        }
        

        [HttpPost("import/excel")]
        public async Task<IActionResult> ImportXlsx(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .xlsx files are supported.");

            await _fileService.ImportXlsxAsync(file.OpenReadStream());
                return Ok("XLSX file imported successfully.");
        }
    }
}