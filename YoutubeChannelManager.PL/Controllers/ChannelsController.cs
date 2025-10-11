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
        private readonly ILogger<ChannelsController> _logger;

        public ChannelsController(
            IFileService fileService, 
            IMediator mediator,
            ILogger<ChannelsController> logger)
        {
            _fileService = fileService;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            try
            {
                _logger.LogInformation("GetAll channels request received with query: {@Query}", query);

                var response = await _mediator.Send(new GetAllChannelsQuery(query));

                _logger.LogInformation("GetAll channels completed. Count: {Count}", response.Count());
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll channels");
                throw;
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation("GetById request received. Id: {ChannelId}", id);

                var response = await _mediator.Send(new GetChannelByIdQuery(id));

                if (response == null)
                {
                    _logger.LogWarning("Channel not found. Id: {ChannelId}", id);
                    return NotFound("Channel not found.");
                }

                _logger.LogInformation("GetById completed. Id: {ChannelId}, Name: {ChannelName}", 
                    response.Id, response.ChannelName);
                    
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById. Id: {ChannelId}", id);
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,Editor")]
        public async Task<IActionResult> Create([FromBody] CreateChannelRequestDto channelDto)
        {
            try
            {
                _logger.LogInformation("Create channel request received. Name: {ChannelName}", 
                    channelDto.ChannelName);

                var createCommand = new CreateChannelCommand(
                    channelDto.ChannelName,
                    channelDto.Category,
                    channelDto.Subscribers,
                    channelDto.IsActive);

                var response = await _mediator.Send(createCommand);

                _logger.LogInformation("Channel created successfully. Id: {ChannelId}, Name: {ChannelName}", 
                    response.Id, response.ChannelName);

                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating channel. Name: {ChannelName}", channelDto.ChannelName);
                throw;
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "SuperAdmin,Admin,Editor")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id, 
            [FromBody] UpdateChannelRequestDto updateChannelRequestDto)
        {
            try
            {
                _logger.LogInformation("Update channel request received. Id: {ChannelId}", id);

                var updateChannelCommand = new UpdateChannelCommand(
                    id,
                    updateChannelRequestDto.ChannelName,
                    updateChannelRequestDto.Category,
                    updateChannelRequestDto.Subscribers,
                    updateChannelRequestDto.IsActive);

                var response = await _mediator.Send(updateChannelCommand);

                if (response == null)
                {
                    _logger.LogWarning("Channel not found for update. Id: {ChannelId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Channel updated successfully. Id: {ChannelId}, Name: {ChannelName}", 
                    response.Id, response.ChannelName);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating channel. Id: {ChannelId}", id);
                throw;
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation("Delete channel request received. Id: {ChannelId}", id);
                
                var response = await _mediator.Send(new DeleteChannelCommand(id));

                if (response == null)
                {
                    _logger.LogWarning("Channel not found for deletion. Id: {ChannelId}", id);
                    return NotFound("Channel not found.");
                }

                _logger.LogInformation("Channel deleted successfully. Id: {ChannelId}, Name: {ChannelName}", 
                    response.Id, response.ChannelName);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting channel. Id: {ChannelId}", id);
                throw;
            }
        }

        [HttpPost("import/csv")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            try
            {
                _logger.LogInformation("CSV import request received. FileName: {FileName}, Size: {FileSize}", 
                    file?.FileName, file?.Length);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("CSV import failed: File is empty or not provided");
                    return BadRequest("File is empty or not provided.");
                }

                if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("CSV import failed: Invalid file extension. FileName: {FileName}", 
                        file.FileName);
                    return BadRequest("Only .csv files are supported.");
                }

                var result = await _mediator.Send(new ImportCsvCommand(file.OpenReadStream()));
                
                _logger.LogInformation("CSV import completed. ImportedCount: {ImportedCount}", 
                    result.ImportedCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CSV import");
                throw;
            }
        }

        [HttpPost("import/excel")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportXlsx(IFormFile file)
        {
            try
            {
                _logger.LogInformation("XLSX import request received. FileName: {FileName}, Size: {FileSize}", 
                    file?.FileName, file?.Length);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("XLSX import failed: File is empty or not provided");
                    return BadRequest("File is empty or not provided.");
                }

                if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("XLSX import failed: Invalid file extension. FileName: {FileName}", 
                        file.FileName);
                    return BadRequest("Only .xlsx files are supported.");
                }

                var result = await _mediator.Send(new ImportXlsxCommand(file.OpenReadStream()));
                
                _logger.LogInformation("XLSX import completed. ImportedCount: {ImportedCount}", 
                    result.ImportedCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during XLSX import");
                throw;
            }
        }

        [HttpPost("import/folder/csv")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportCsvFolder([FromBody] FolderPathRequestDto request)
        {
            try
            {
                _logger.LogInformation("CSV folder import request received. FolderPath: {FolderPath}", 
                    request?.FolderPath);

                if (request is null || string.IsNullOrWhiteSpace(request.FolderPath))
                {
                    _logger.LogWarning("CSV folder import failed: FolderPath is empty");
                    return BadRequest("FolderPath is empty or not provided.");
                }

                if (!Directory.Exists(request.FolderPath))
                {
                    _logger.LogWarning("CSV folder import failed: Folder not found. Path: {FolderPath}", 
                        request.FolderPath);
                    return BadRequest("Folder not found.");
                }

                var anyCsv = Directory.EnumerateFiles(request.FolderPath, "*.csv", SearchOption.TopDirectoryOnly).Any();
                if (!anyCsv)
                {
                    _logger.LogWarning("CSV folder import failed: No CSV files found. Path: {FolderPath}", 
                        request.FolderPath);
                    return BadRequest("No .csv files found in the provided folder.");
                }

                var result = await _mediator.Send(new ImportCsvFolderCommand(request.FolderPath));
                
                _logger.LogInformation(
                    "CSV folder import completed. ImportedCount: {ImportedCount}, ProcessedFiles: {ProcessedFiles}", 
                    result.ImportedCount, result.ProcessedFileCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CSV folder import");
                throw;
            }
        }

        [HttpPost("import/folder/xlsx")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ImportXlsxFolder([FromBody] FolderPathRequestDto request)
        {
            try
            {
                _logger.LogInformation("XLSX folder import request received. FolderPath: {FolderPath}", 
                    request?.FolderPath);

                if (request is null || string.IsNullOrWhiteSpace(request.FolderPath))
                {
                    _logger.LogWarning("XLSX folder import failed: FolderPath is empty");
                    return BadRequest("FolderPath is empty or not provided.");
                }

                if (!Directory.Exists(request.FolderPath))
                {
                    _logger.LogWarning("XLSX folder import failed: Folder not found. Path: {FolderPath}", 
                        request.FolderPath);
                    return BadRequest("Folder not found.");
                }

                var anyXlsx = Directory.EnumerateFiles(request.FolderPath, "*.xlsx", SearchOption.TopDirectoryOnly).Any();
                if (!anyXlsx)
                {
                    _logger.LogWarning("XLSX folder import failed: No XLSX files found. Path: {FolderPath}", 
                        request.FolderPath);
                    return BadRequest("No .xlsx files found in the provided folder.");
                }

                var result = await _mediator.Send(new ImportXlsxFolderCommand(request.FolderPath));
                
                _logger.LogInformation(
                    "XLSX folder import completed. ImportedCount: {ImportedCount}, ProcessedFiles: {ProcessedFiles}", 
                    result.ImportedCount, result.ProcessedFileCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during XLSX folder import");
                throw;
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportChannels(
            [FromQuery] QueryObject query,
            [FromQuery] string format = "csv")
        {
            try
            {
                _logger.LogInformation("Export channels request received. Format: {Format}, Query: {@Query}", 
                    format, query);

                var result = await _mediator.Send(new ExportChannelsQuery(query, format));
                
                _logger.LogInformation("Export completed successfully. Format: {Format}, FileName: {FileName}", 
                    format, result.FileName);

                return File(result.FileContent, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during channel export. Format: {Format}", format);
                throw;
            }
        }
    }
}