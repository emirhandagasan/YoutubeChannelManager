using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsv
{
    public class ImportCsvHandler : IRequestHandler<ImportCsvCommand>
    {
        private readonly IFileService _fileService;

        public ImportCsvHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Unit> Handle(ImportCsvCommand request, CancellationToken cancellationToken)
        {
            await _fileService.ImportCsvAsync(request.FileStream);
            return Unit.Value;
        }
    }
}
