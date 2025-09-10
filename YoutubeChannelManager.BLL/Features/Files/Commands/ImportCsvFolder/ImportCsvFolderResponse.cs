using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeChannelManager.BLL.Features.Files.Commands.ImportCsvFolder
{
    public class ImportCsvFolderResponse
    {
        public bool Success { get; set; }
        public int ImportedCount { get; set; }
        public int ProcessedFileCount { get; set; }
        public string Message { get; set; }
    }
}