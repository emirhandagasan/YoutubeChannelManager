using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
            var response = await _mediator.Send(new GetAllChannelsQuery(query));
            return Ok(response);
        }



        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _mediator.Send(new GetChannelByIdQuery(id));

            if (response == null)
                return NotFound("Channel not found.");

            return Ok(response);
        }



        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,Editor")]
        public async Task<IActionResult> Create([FromBody] CreateChannelRequestDto channelDto)
        {
            var createCommand = new CreateChannelCommand
            (
            channelDto.ChannelName,
            channelDto.Category,
            channelDto.Subscribers,
            channelDto.IsActive);

            var response = await _mediator.Send(createCommand);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }



        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "SuperAdmin,Admin,Editor")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateChannelRequestDto updateChannelRequestDto)
        {
            var updateChannelCommand = new UpdateChannelCommand(
                id,
                updateChannelRequestDto.ChannelName,
                updateChannelRequestDto.Category,
                updateChannelRequestDto.Subscribers,
                updateChannelRequestDto.IsActive);

            var response = await _mediator.Send(updateChannelCommand);

            if (response == null)
                return NotFound();

            return Ok(response);
        }



        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _mediator.Send(new DeleteChannelCommand(id));

            if (response == null)
                return NotFound("Channel not found.");

            return Ok(response);
        }



        [HttpPost("import/csv")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .csv files are supported.");

            var result = await _mediator.Send(new ImportCsvCommand(file.OpenReadStream()));
            return Ok(result);
        }



        [HttpPost("import/excel")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportXlsx(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .xlsx files are supported.");

            var result = await _mediator.Send(new ImportXlsxCommand(file.OpenReadStream()));
            return Ok(result);
        }



        [HttpPost("import/folder/csv")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportCsvFolder([FromBody] FolderPathRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.FolderPath))
                return BadRequest("FolderPath is empty or not provided.");

            if (!Directory.Exists(request.FolderPath))
                return BadRequest("Folder not found.");


            var anyCsv = Directory.EnumerateFiles(request.FolderPath, "*.csv", SearchOption.TopDirectoryOnly).Any();
            if (!anyCsv)
                return BadRequest("No .csv files found in the provided folder.");

            var result = await _mediator.Send(new ImportCsvFolderCommand(request.FolderPath));
            return Ok(result);
        }



        [HttpPost("import/folder/xlsx")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportXlsxFolder([FromBody] FolderPathRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.FolderPath))
                return BadRequest("FolderPath is empty or not provided.");

            if (!Directory.Exists(request.FolderPath))
                return BadRequest("Folder not found.");

            var anyXlsx = Directory.EnumerateFiles(request.FolderPath, "*.xlsx", SearchOption.TopDirectoryOnly).Any();
            if (!anyXlsx)
                return BadRequest("No .xlsx files found in the provided folder.");

            var result = await _mediator.Send(new ImportXlsxFolderCommand(request.FolderPath));
            return Ok(result);
        }



        [HttpGet("export")]
        public async Task<IActionResult> ExportChannels(
        [FromQuery] QueryObject query,
        [FromQuery] string format = "csv")
        {
            var result = await _mediator.Send(new ExportChannelsQuery(query, format));
            
            return File(result.FileContent, result.ContentType, result.FileName);
        }
    }
}