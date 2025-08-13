using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeChannelManager.Interfaces
{
    public interface IFileService
    {
        Task ImportCsvAsync(Stream fileStream);
        Task ImportXlsxAsync(Stream fileStream);
    }
}