using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsvFolder
{
    public class ImportCsvFolderCommandHandler : IRequestHandler<ImportCsvFolderCommand, ImportCsvFolderResponse>
    {
        private readonly IFileService _fileService;

        public ImportCsvFolderCommandHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ImportCsvFolderResponse> Handle(ImportCsvFolderCommand request, CancellationToken cancellationToken)
        {
            var (importedCount, processedFileCount) = await _fileService.ImportCsvFolderAsync(request.FolderPath);

            return new ImportCsvFolderResponse
            {
                Success = true,
                ImportedCount = importedCount,
                ProcessedFileCount = processedFileCount,
                Message = $"{importedCount} channels were imported from {processedFileCount} .csv files in the folder."
            };
        }
    }
}
