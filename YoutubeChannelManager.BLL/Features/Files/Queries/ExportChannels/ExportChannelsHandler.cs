using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Queries.ExportChannels
{
    public class ExportChannelsHandler : IRequestHandler<ExportChannelsQuery, byte[]>
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IFileService _fileService;

        public ExportChannelsHandler(IChannelRepository channelRepository, IFileService fileService)
        {
            _channelRepository = channelRepository;
            _fileService = fileService;
        }

        public async Task<byte[]> Handle(ExportChannelsQuery request, CancellationToken cancellationToken)
        {
            var channels = await _channelRepository.GetAllAsync(request.Query);

            return request.Format.ToLower() switch
            {
                "csv" => Encoding.UTF8.GetBytes(_fileService.ExportChannelsToCsv(channels)),
                "xlsx" => _fileService.ExportChannelsToXlsx(channels),
                _ => throw new ArgumentException("Invalid format. Please type 'csv' or 'xlsx'.")
            };
        }
    }
}
