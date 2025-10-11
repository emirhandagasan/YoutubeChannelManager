using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ImportCsvFolderCommandHandler> _logger;

        public ImportCsvFolderCommandHandler(
            IFileService fileService,
            ILogger<ImportCsvFolderCommandHandler> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<ImportCsvFolderResponse> Handle(ImportCsvFolderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing CSV folder import command. Path: {FolderPath}", request.FolderPath);

                var (importedCount, processedFileCount) = await _fileService.ImportCsvFolderAsync(request.FolderPath);

                _logger.LogInformation(
                    "CSV folder import command completed. Imported: {ImportedCount}, Files: {FileCount}", 
                    importedCount, processedFileCount);

                return new ImportCsvFolderResponse
                {
                    Success = true,
                    ImportedCount = importedCount,
                    ProcessedFileCount = processedFileCount,
                    Message = $"{importedCount} channels were imported from {processedFileCount} .csv files in the folder."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CSV folder import command. Path: {FolderPath}", request.FolderPath);
                throw;
            }
        }
    }
}

