using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Queries.ExportChannels
{
    public class ExportChannelsQueryHandler : IRequestHandler<ExportChannelsQuery, ExportChannelsResponse>
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<ExportChannelsQueryHandler> _logger;

        public ExportChannelsQueryHandler(
            IChannelRepository channelRepository, 
            IFileService fileService,
            ILogger<ExportChannelsQueryHandler> logger)
        {
            _channelRepository = channelRepository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<ExportChannelsResponse> Handle(ExportChannelsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Exporting channels. Format: {Format}, Query: {@Query}", 
                    request.Format, request.Query);

                var channels = await _channelRepository.GetAllAsync(request.Query);
                var channelList = channels.ToList();

                _logger.LogInformation("Retrieved {Count} channels for export", channelList.Count);

                byte[] fileContent;
                string fileName;
                string contentType;

                if (request.Format.ToLower() == "xlsx")
                {
                    fileContent = _fileService.ExportChannelsToXlsx(channelList);
                    fileName = "channels.xlsx";
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    _logger.LogInformation("Channels exported as XLSX. Count: {Count}", channelList.Count);
                }
                else
                {
                    var csv = _fileService.ExportChannelsToCsv(channelList);
                    fileContent = System.Text.Encoding.UTF8.GetBytes(csv);
                    fileName = "channels.csv";
                    contentType = "text/csv";
                    
                    _logger.LogInformation("Channels exported as CSV. Count: {Count}", channelList.Count);
                }

                return new ExportChannelsResponse
                {
                    FileContent = fileContent,
                    FileName = fileName,
                    ContentType = contentType,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting channels. Format: {Format}", request.Format);
                throw;
            }
        }
    }
}
