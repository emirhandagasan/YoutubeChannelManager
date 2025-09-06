using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YoutubeChannelManager.BLL.DTOs;
using YoutubeChannelManager.BLL.Features.Channels.Commands.CreateChannel;
using YoutubeChannelManager.BLL.Features.Channels.Commands.DeleteChannel;
using YoutubeChannelManager.BLL.Features.Channels.Commands.UpdateChannel;
using YoutubeChannelManager.BLL.Features.Channels.Queries.GetAllChannel;
using YoutubeChannelManager.BLL.Features.Channels.Queries.GetByIdChannel;
using YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsv;
using YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsvFolder;
using YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsx;
using YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsxFolder;
using YoutubeChannelManager.BLL.Features.Files.Queries.ExportChannels;
using YoutubeChannelManager.BLL.Helpers;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;


namespace YoutubeChannelManager.Controllers
{

    [ApiController]
    [Route("api/channels")]
    public class ChannelsController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IMediator _mediator;

        public ChannelsController(IFileService fileService, IMediator mediator)
        {
            _fileService = fileService;
            _mediator = mediator;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var result = await _mediator.Send(new GetAllChannelsQuery(query));
            return Ok(result);
        }



        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var channel = await _mediator.Send(new GetChannelByIdQuery(id));

            if (channel == null)
                return NotFound();

            return Ok(channel);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChannelRequestDto channelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createChannelCommand = new CreateChannelCommand(
                channelDto.ChannelName,
                channelDto.Category,
                channelDto.Subscribers,
                channelDto.IsActive
            );

            var created = await _mediator.Send(createChannelCommand);

            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }



        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateChannelRequestDto updateChannelRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updateChannelCommand = new UpdateChannelCommand(
                id, 
                updateChannelRequestDto.ChannelName, 
                updateChannelRequestDto.Category, 
                updateChannelRequestDto.Subscribers, 
                updateChannelRequestDto.IsActive);

            var updated = await _mediator.Send(updateChannelCommand);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }



        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deleted = await _mediator.Send(new DeleteChannelCommand(id));

            if (deleted == null)
                return NotFound();

            return NoContent();
        }



        [HttpPost("import/csv")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .csv files are supported.");

            await _mediator.Send(new ImportCsvCommand(file.OpenReadStream()));
            return Ok(".csv file imported successfully.");
        }



        [HttpPost("import/excel")]
        public async Task<IActionResult> ImportXlsx(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .xlsx files are supported.");

            await _mediator.Send(new ImportXlsxCommand(file.OpenReadStream()));
            return Ok(".xlsx file imported successfully.");
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

            await _mediator.Send(new ImportCsvFolderCommand(request.FolderPath));
            return Ok(".csv files from folder imported successfully.");
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

            await _mediator.Send(new ImportXlsxFolderCommand(request.FolderPath));
            return Ok(".xlsx files from folder imported successfully.");
        }



        [HttpGet("export")]
        public async Task<IActionResult> ExportChannels(
        [FromQuery] QueryObject query,
        [FromQuery] string format = "csv")
        {
            var bytes = await _mediator.Send(new ExportChannelsQuery(query, format));
            var contentType = format.ToLower() == "csv"
                ? "text/csv"
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = format.ToLower() == "csv" ? "channels.csv" : "channels.xlsx";

            return File(bytes, contentType, fileName);
        }
    }
}