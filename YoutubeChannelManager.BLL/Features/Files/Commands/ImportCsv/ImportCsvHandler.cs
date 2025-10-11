using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsv
{
    public class ImportCsvCommandHandler : IRequestHandler<ImportCsvCommand, ImportCsvResponse>
    {
        private readonly IFileService _fileService;
        private readonly ILogger<ImportCsvCommandHandler> _logger;

        public ImportCsvCommandHandler(
            IFileService fileService,
            ILogger<ImportCsvCommandHandler> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<ImportCsvResponse> Handle(ImportCsvCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing CSV import command");

                var count = await _fileService.ImportCsvAsync(request.FileStream);

                _logger.LogInformation("CSV import command completed. Imported: {Count}", count);

                return new ImportCsvResponse
                {
                    Success = true,
                    ImportedCount = count,
                    Message = $"{count} channels imported from CSV."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CSV import command");
                throw;
            }
        }
    }
}

