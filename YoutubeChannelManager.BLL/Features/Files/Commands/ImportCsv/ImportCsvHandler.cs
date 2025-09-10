using MediatR;
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

        public ImportCsvCommandHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ImportCsvResponse> Handle(ImportCsvCommand request, CancellationToken cancellationToken)
        {
            var count = await _fileService.ImportCsvAsync(request.FileStream);

            return new ImportCsvResponse
            {
                Success = true,
                ImportedCount = count,
                Message = $"{count} channels imported from CSV."
            };
        }
    }
}
