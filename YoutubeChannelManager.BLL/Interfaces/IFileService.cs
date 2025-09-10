using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Interfaces
{
    public interface IFileService
    {
        Task<int> ImportCsvAsync(Stream fileStream);
        Task<int> ImportXlsxAsync(Stream fileStream);
        Task<(int ImportedCount, int ProcessedFileCount)> ImportCsvFolderAsync(string folderPath);
        Task<(int ImportedCount, int ProcessedFileCount)> ImportXlsxFolderAsync(string folderPath);
        string ExportChannelsToCsv(IEnumerable<Channel> channels);
        byte[] ExportChannelsToXlsx(IEnumerable<Channel> channels);
    }
}
