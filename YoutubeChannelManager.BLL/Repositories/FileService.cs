using ClosedXML.Excel;
using CsvHelper;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.BLL.Repositories
{
    public class FileService : IFileService
    {
        private readonly IChannelRepository _channelRepository;
        public FileService(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }


        public async Task<int> ImportCsvAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Channel>().ToList();

            int importedCount = 0;

            foreach (var record in records)
            {
                bool exists = await _channelRepository.ExistsByNameAsync(record.ChannelName);

                if (exists)
                    continue;

                if (string.IsNullOrWhiteSpace(record.ChannelName))
                    continue;

                record.Id = Guid.NewGuid();
                record.CreatedAt = DateTime.UtcNow;

                await _channelRepository.CreateAsync(record);
                importedCount++;
            }

            return importedCount;
        }


        public async Task<int> ImportXlsxAsync(Stream fileStream)
        {
            var records = MiniExcel.Query<Channel>(fileStream).ToList();
            int importedCount = 0;

            foreach (var record in records)
            {
                bool exists = await _channelRepository.ExistsByNameAsync(record.ChannelName);

                if (exists)
                    continue;

                if (string.IsNullOrWhiteSpace(record.ChannelName))
                    continue;

                record.Id = Guid.NewGuid();
                record.CreatedAt = DateTime.UtcNow;

                await _channelRepository.CreateAsync(record);
                importedCount++;
            }

            return importedCount;
        }


        public async Task<(int ImportedCount, int ProcessedFileCount)> ImportCsvFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            int importedCount = 0;
            int processedFileCount = 0;

            foreach (var file in Directory.EnumerateFiles(folderPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                using var reader = new StreamReader(file);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<Channel>().ToList();
                bool fileHadValidChannels = false;

                foreach (var record in records)
                {
                    if (string.IsNullOrWhiteSpace(record.ChannelName))
                        continue;

                    if (await _channelRepository.ExistsByNameAsync(record.ChannelName))
                        continue;

                    record.Id = Guid.NewGuid();
                    record.CreatedAt = DateTime.UtcNow;

                    await _channelRepository.CreateAsync(record);
                    
                    importedCount++;
                    fileHadValidChannels = true;
                }

                if (fileHadValidChannels)
                    processedFileCount++;
            }

            return (importedCount, processedFileCount);
        }


        public async Task<(int ImportedCount, int ProcessedFileCount)> ImportXlsxFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            int importedCount = 0;
            int processedFileCount = 0;

            foreach (var file in Directory.EnumerateFiles(folderPath, "*.xlsx", SearchOption.TopDirectoryOnly))
            {
                using var reader = File.OpenRead(file);
                var records = MiniExcel.Query<Channel>(reader).ToList();
                bool fileHadValidChannels = false;

                foreach (var record in records)
                {
                    if (await _channelRepository.ExistsByNameAsync(record.ChannelName))
                        continue;

                    if (string.IsNullOrWhiteSpace(record.ChannelName))
                        continue;

                    record.Id = Guid.NewGuid();
                    record.CreatedAt = DateTime.UtcNow;

                    await _channelRepository.CreateAsync(record);
                    importedCount++;
                    fileHadValidChannels = true;
                }

                if (fileHadValidChannels)
                    processedFileCount++;
            }

            return (importedCount, processedFileCount);
        }



        public string ExportChannelsToCsv(IEnumerable<Channel> channels)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Id,ChannelName,Category,Subscribers,IsActive");
            foreach (var c in channels)
            {
                stringBuilder.AppendLine($"{c.Id},{c.ChannelName},{c.Category},{c.Subscribers},{c.IsActive}");
            }
            return stringBuilder.ToString();
        }



        public byte[] ExportChannelsToXlsx(IEnumerable<Channel> channels)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Channels");
            ws.FirstRow().Cell(1).Value = "Id";
            ws.FirstRow().Cell(2).Value = "ChannelName";
            ws.FirstRow().Cell(3).Value = "Category";
            ws.FirstRow().Cell(4).Value = "Subscribers";
            ws.FirstRow().Cell(5).Value = "IsActive";

            int row = 2;
            foreach (var channel in channels)
            {
                ws.Cell(row, 1).Value = channel.Id.ToString();
                ws.Cell(row, 2).Value = channel.ChannelName;
                ws.Cell(row, 3).Value = channel.Category;
                ws.Cell(row, 4).Value = channel.Subscribers;
                ws.Cell(row, 5).Value = channel.IsActive;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
