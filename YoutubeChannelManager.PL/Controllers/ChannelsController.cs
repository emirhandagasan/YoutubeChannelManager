using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YoutubeChannelManager.BLL.DTOs;
using YoutubeChannelManager.BLL.Helpers;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;


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
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var result = await _channelRepository.GetAllAsync(query);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


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


        [HttpPost("import/folder/csv")]
        public async Task<IActionResult> ImportCsvFolder([FromBody] FolderPathRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.FolderPath))
                return BadRequest("FolderPath is empty or not provided.");

            if (!Directory.Exists(request.FolderPath))
                return BadRequest("Folder not found.");


            var anyCsv = Directory.EnumerateFiles(request.FolderPath, "*.csv", SearchOption.TopDirectoryOnly).Any();
            if (!anyCsv)
                return BadRequest("No .csv files found in the provided folder.");

            await _fileService.ImportCsvFolderAsync(request.FolderPath);
            return Ok("CSV files from folder imported successfully.");
        }


        [HttpPost("import/folder/xlsx")]
        public async Task<IActionResult> ImportXlsxFolder([FromBody] FolderPathRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.FolderPath))
                return BadRequest("FolderPath is empty or not provided.");

            if (!Directory.Exists(request.FolderPath))
                return BadRequest("Folder not found.");

            var anyXlsx = Directory.EnumerateFiles(request.FolderPath, "*.xlsx", SearchOption.TopDirectoryOnly).Any();
            if (!anyXlsx)
                return BadRequest("No .xlsx files found in the provided folder.");

            await _fileService.ImportXlsxFolderAsync(request.FolderPath);
            return Ok("XLSX files from folder imported successfully.");
        }


        [HttpGet("export")]
        public async Task<IActionResult> ExportChannels(
        [FromQuery] QueryObject query,
        [FromQuery] string format = "csv")
        {
            var channels = await _channelRepository.GetAllAsync(query);

            if (format.ToLower() == "csv")
            {
                var csv = _fileService.ExportChannelsToCsv(channels);
                var bytes = Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", "channels.csv");
            }
            else if (format.ToLower() == "xlsx")
            {
                var xlsxBytes = _fileService.ExportChannelsToXlsx(channels);
                return File(xlsxBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "channels.xlsx");
            }
            else
                return BadRequest("Invalid format. Use 'csv' or 'xlsx'.");
        }
    }
}