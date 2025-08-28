using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportXlsxFolder
{
    public record ImportXlsxFolderCommand(string FolderPath) : IRequest;
}
