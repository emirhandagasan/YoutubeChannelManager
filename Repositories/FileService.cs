using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using MiniExcelLibs;
using YoutubeChannelManager.Interfaces;
using YoutubeChannelManager.Models;

namespace YoutubeChannelManager.Repositories
{
    public class FileService : IFileService
    {
        private readonly IChannelRepository _channelRepository;
        public FileService(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }


        public async Task ImportCsvAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Channel>().ToList();
            foreach (var record in records)
            {
                bool exists = await _channelRepository.ExistsByNameAsync(record.ChannelName);

                if (string.IsNullOrWhiteSpace(record.ChannelName))
                    continue;

                if (exists)
                    continue;

                record.Id = Guid.NewGuid();
                record.CreatedAt = DateTime.UtcNow;
                await _channelRepository.CreateAsync(record);
            }
        }

        public async Task ImportXlsxAsync(Stream fileStream)
        {
            var records = MiniExcel.Query<Channel>(fileStream).ToList();

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.ChannelName))
                    continue;

                if (await _channelRepository.ExistsByNameAsync(record.ChannelName))
                    continue;

                record.Id = Guid.NewGuid();
                record.CreatedAt = DateTime.UtcNow;
                await _channelRepository.CreateAsync(record);
            }
        }


        public async Task ImportCsvFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            foreach (var file in Directory.EnumerateFiles(folderPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                using var reader = new StreamReader(file);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<Channel>().ToList();
                foreach (var record in records)
                {
                    if (string.IsNullOrWhiteSpace(record.ChannelName))
                        continue;

                    if (await _channelRepository.ExistsByNameAsync(record.ChannelName))
                        continue;

                    record.Id = Guid.NewGuid();
                    record.CreatedAt = DateTime.UtcNow;
                    await _channelRepository.CreateAsync(record);
                }
            }
        }
        

        public async Task ImportXlsxFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            foreach (var file in Directory.EnumerateFiles(folderPath, "*.xlsx", SearchOption.TopDirectoryOnly))
            {
                using var reader = File.OpenRead(file);
                var records = MiniExcel.Query<Channel>(reader).ToList();

                foreach (var record in records)
                {
                    if (string.IsNullOrWhiteSpace(record.ChannelName))
                        continue;

                    if (await _channelRepository.ExistsByNameAsync(record.ChannelName))
                        continue;

                    record.Id = Guid.NewGuid();
                    record.CreatedAt = DateTime.UtcNow;
                    await _channelRepository.CreateAsync(record);
                }
            }
        }
    }
}