using MediatR;
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

        public ExportChannelsQueryHandler(IChannelRepository channelRepository, IFileService fileService)
        {
            _channelRepository = channelRepository;
            _fileService = fileService;
        }

        public async Task<ExportChannelsResponse> Handle(ExportChannelsQuery request, CancellationToken cancellationToken)
        {
            var channels = await _channelRepository.GetAllAsync(request.Query);

            byte[] fileContent;
            string fileName;
            string contentType;

            if (request.Format.ToLower() == "xlsx")
            {
                fileContent = _fileService.ExportChannelsToXlsx(channels);
                fileName = "channels.xlsx";
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else
            {
                var csv = _fileService.ExportChannelsToCsv(channels);
                fileContent = System.Text.Encoding.UTF8.GetBytes(csv);
                fileName = "channels.csv";
                contentType = "text/csv";
            }

            return new ExportChannelsResponse
            {
                FileContent = fileContent,
                FileName = fileName,
                ContentType = contentType,
            };
        }
    }
}
