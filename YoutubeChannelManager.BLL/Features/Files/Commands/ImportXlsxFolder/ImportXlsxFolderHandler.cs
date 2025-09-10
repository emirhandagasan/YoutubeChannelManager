using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsxFolder
{
    public class ImportXlsxFolderCommandHandler : IRequestHandler<ImportXlsxFolderCommand, ImportXlsxFolderResponse>
    {
        private readonly IFileService _fileService;

        public ImportXlsxFolderCommandHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ImportXlsxFolderResponse> Handle(ImportXlsxFolderCommand request, CancellationToken cancellationToken)
        {
            var (importedCount, processedFileCount) = await _fileService.ImportXlsxFolderAsync(request.FolderPath);

            return new ImportXlsxFolderResponse
            {
                Success = true,
                ImportedCount = importedCount,
                ProcessedFileCount = processedFileCount,
                Message = $"{importedCount} channels were imported from {processedFileCount} .xlsx files in the folder."
            };
        }
    }
}
