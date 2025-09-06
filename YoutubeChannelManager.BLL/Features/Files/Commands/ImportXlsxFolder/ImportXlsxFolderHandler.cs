using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsxFolder
{
    public class ImportXlsxFolderHandler : IRequestHandler<ImportXlsxFolderCommand>
    {
        private readonly IFileService _fileService;

        public ImportXlsxFolderHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Unit> Handle(ImportXlsxFolderCommand request, CancellationToken cancellationToken)
        {
            await _fileService.ImportXlsxFolderAsync(request.FolderPath);
            return Unit.Value;
        }
    }
}
