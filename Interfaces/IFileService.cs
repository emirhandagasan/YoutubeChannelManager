using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeChannelManager.Models;

namespace YoutubeChannelManager.Interfaces
{
    public interface IFileService
    {
        Task ImportCsvAsync(Stream fileStream);
        Task ImportXlsxAsync(Stream fileStream);
        Task ImportCsvFolderAsync(string folderPath);
        Task ImportXlsxFolderAsync(string folderPath); 
        string ExportChannelsToCsv(IEnumerable<Channel> channels);
        byte[] ExportChannelsToXlsx(IEnumerable<Channel> channels);
    }
}