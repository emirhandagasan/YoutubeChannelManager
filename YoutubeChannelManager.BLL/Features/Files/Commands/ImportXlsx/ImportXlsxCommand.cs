using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsx
{
    public record ImportXlsxCommand(Stream FileStream) : IRequest<ImportXlsxResponse>;
}

