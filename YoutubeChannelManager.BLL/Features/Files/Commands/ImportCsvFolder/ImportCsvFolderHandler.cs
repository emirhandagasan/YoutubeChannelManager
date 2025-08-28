using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsvFolder
{
    public class ImportCsvFolderHandler : IRequestHandler<ImportCsvFolderCommand>
    {
        private readonly IFileService _fileService;

        public ImportCsvFolderHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Unit> Handle(ImportCsvFolderCommand request, CancellationToken cancellationToken)
        {
            await _fileService.ImportCsvFolderAsync(request.FolderPath);
            return Unit.Value;
        }
    }
}
