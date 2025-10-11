using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsx
{
   public class ImportXlsxCommandHandler : IRequestHandler<ImportXlsxCommand, ImportXlsxResponse>
    {
        private readonly IFileService _fileService;
        private readonly ILogger<ImportXlsxCommandHandler> _logger;

        public ImportXlsxCommandHandler(
            IFileService fileService,
            ILogger<ImportXlsxCommandHandler> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<ImportXlsxResponse> Handle(ImportXlsxCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing XLSX import command");

                var count = await _fileService.ImportXlsxAsync(request.FileStream);

                _logger.LogInformation("XLSX import command completed. Imported: {Count}", count);

                return new ImportXlsxResponse
                {
                    Success = true,
                    ImportedCount = count,
                    Message = $"{count} channels imported from XLSX."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in XLSX import command");
                throw;
            }
        }
    }
}
