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
    }
}