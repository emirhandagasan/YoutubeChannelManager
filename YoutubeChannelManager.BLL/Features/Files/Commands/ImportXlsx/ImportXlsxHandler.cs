using MediatR;
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

        public ImportXlsxCommandHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ImportXlsxResponse> Handle(ImportXlsxCommand request, CancellationToken cancellationToken)
        {
            var count = await _fileService.ImportXlsxAsync(request.FileStream);

            return new ImportXlsxResponse
            {
                Success = true,
                ImportedCount = count,
                Message = $"{count} channels imported from XLSX."
            };
        }
    }
}
