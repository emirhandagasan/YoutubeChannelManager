using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ImportXlsxFolderCommandHandler> _logger;

        public ImportXlsxFolderCommandHandler(
            IFileService fileService,
            ILogger<ImportXlsxFolderCommandHandler> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<ImportXlsxFolderResponse> Handle(ImportXlsxFolderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing XLSX folder import command. Path: {FolderPath}", request.FolderPath);

                var (importedCount, processedFileCount) = await _fileService.ImportXlsxFolderAsync(request.FolderPath);

                _logger.LogInformation(
                    "XLSX folder import command completed. Imported: {ImportedCount}, Files: {FileCount}",
                    importedCount, processedFileCount);

                return new ImportXlsxFolderResponse
                {
                    Success = true,
                    ImportedCount = importedCount,
                    ProcessedFileCount = processedFileCount,
                    Message = $"{importedCount} channels were imported from {processedFileCount} .xlsx files in the folder."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in XLSX folder import command. Path: {FolderPath}", request.FolderPath);
                throw;
            }
        }
    }
}

