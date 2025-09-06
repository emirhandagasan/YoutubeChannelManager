using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsx
{
    public class ImportXlsxHandler : IRequestHandler<ImportXlsxCommand>
    {
        private readonly IFileService _fileService;

        public ImportXlsxHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Unit> Handle(ImportXlsxCommand request, CancellationToken cancellationToken)
        {
            await _fileService.ImportXlsxAsync(request.FileStream);
            return Unit.Value;
        }
    }
}
