using ClosedXML.Excel;
using CsvHelper;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<FileService> _logger;

        public FileService(IChannelRepository channelRepository, ILogger<FileService> logger)
        {
            _channelRepository = channelRepository;
            _logger = logger;
        }

        public async Task<int> ImportCsvAsync(Stream fileStream)
        {
            try
            {
                _logger.LogInformation("Starting CSV import");

                using var reader = new StreamReader(fileStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<Channel>().ToList();

                _logger.LogInformation("Found {RecordCount} records in CSV", records.Count);

                int importedCount = 0;

                foreach (var record in records)
                {
                    bool exists = await _channelRepository.ExistsByNameAsync(record.ChannelName);

                    if (exists)
                    {
                        _logger.LogDebug("Skipping duplicate channel: {ChannelName}", record.ChannelName);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.ChannelName))
                    {
                        _logger.LogWarning("Skipping record with empty ChannelName");
                        continue;
                    }

                    record.Id = Guid.NewGuid();
                    record.CreatedAt = DateTime.UtcNow;

                    await _channelRepository.CreateAsync(record);
                    importedCount++;
                }

                _logger.LogInformation("CSV import completed. Imported {ImportedCount} out of {TotalCount} records", 
                    importedCount, records.Count);

                return importedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CSV import");
                throw;
            }
        }

        public async Task<int> ImportXlsxAsync(Stream fileStream)
        {
            try
            {
                _logger.LogInformation("Starting XLSX import");

                var records = MiniExcel.Query<Channel>(fileStream).ToList();
                _logger.LogInformation("Found {RecordCount} records in XLSX", records.Count);

                int importedCount = 0;

                foreach (var record in records)
                {
                    bool exists = await _channelRepository.ExistsByNameAsync(record.ChannelName);

                    if (exists)
                    {
                        _logger.LogDebug("Skipping duplicate channel: {ChannelName}", record.ChannelName);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.ChannelName))
                    {
                        _logger.LogWarning("Skipping record with empty ChannelName");
                        continue;
                    }

                    record.Id = Guid.NewGuid();
                    record.CreatedAt = DateTime.UtcNow;

                    await _channelRepository.CreateAsync(record);
                    importedCount++;
                }

                _logger.LogInformation("XLSX import completed. Imported {ImportedCount} out of {TotalCount} records",
                    importedCount, records.Count);

                return importedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during XLSX import");
                throw;
            }
        }

        public async Task<(int ImportedCount, int ProcessedFileCount)> ImportCsvFolderAsync(string folderPath)
        {
            try
            {
                _logger.LogInformation("Starting CSV folder import from: {FolderPath}", folderPath);

                if (!Directory.Exists(folderPath))
                {
                    _logger.LogError("Folder not found: {FolderPath}", folderPath);
                    throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
                }

                int importedCount = 0;
                int processedFileCount = 0;
                var csvFiles = Directory.EnumerateFiles(folderPath, "*.csv", SearchOption.TopDirectoryOnly).ToList();

                _logger.LogInformation("Found {FileCount} CSV files in folder", csvFiles.Count);

                foreach (var file in csvFiles)
                {
                    _logger.LogInformation("Processing file: {FileName}", Path.GetFileName(file));

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
                    {
                        processedFileCount++;
                        _logger.LogInformation("File processed successfully: {FileName}", Path.GetFileName(file));
                    }
                }

                _logger.LogInformation(
                    "CSV folder import completed. Processed {ProcessedFileCount} files, imported {ImportedCount} channels",
                    processedFileCount, importedCount);

                return (importedCount, processedFileCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CSV folder import from: {FolderPath}", folderPath);
                throw;
            }
        }

        public async Task<(int ImportedCount, int ProcessedFileCount)> ImportXlsxFolderAsync(string folderPath)
        {
            try
            {
                _logger.LogInformation("Starting XLSX folder import from: {FolderPath}", folderPath);

                if (!Directory.Exists(folderPath))
                {
                    _logger.LogError("Folder not found: {FolderPath}", folderPath);
                    throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
                }

                int importedCount = 0;
                int processedFileCount = 0;
                var xlsxFiles = Directory.EnumerateFiles(folderPath, "*.xlsx", SearchOption.TopDirectoryOnly).ToList();

                _logger.LogInformation("Found {FileCount} XLSX files in folder", xlsxFiles.Count);

                foreach (var file in xlsxFiles)
                {
                    _logger.LogInformation("Processing file: {FileName}", Path.GetFileName(file));

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
                    {
                        processedFileCount++;
                        _logger.LogInformation("File processed successfully: {FileName}", Path.GetFileName(file));
                    }
                }

                _logger.LogInformation(
                    "XLSX folder import completed. Processed {ProcessedFileCount} files, imported {ImportedCount} channels",
                    processedFileCount, importedCount);

                return (importedCount, processedFileCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during XLSX folder import from: {FolderPath}", folderPath);
                throw;
            }
        }

        public string ExportChannelsToCsv(IEnumerable<Channel> channels)
        {
            try
            {
                var channelList = channels.ToList();

                _logger.LogInformation("Exporting {Count} channels to CSV", channelList.Count);

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Id,ChannelName,Category,Subscribers,IsActive");
                foreach (var c in channelList)
                {
                    stringBuilder.AppendLine($"{c.Id},{c.ChannelName},{c.Category},{c.Subscribers},{c.IsActive}");
                }

                _logger.LogInformation("CSV export completed successfully");
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CSV export");
                throw;
            }
        }

        public byte[] ExportChannelsToXlsx(IEnumerable<Channel> channels)
        {
            try
            {
                var channelList = channels.ToList();

                _logger.LogInformation("Exporting {Count} channels to XLSX", channelList.Count);

                using var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Channels");
                ws.FirstRow().Cell(1).Value = "Id";
                ws.FirstRow().Cell(2).Value = "ChannelName";
                ws.FirstRow().Cell(3).Value = "Category";
                ws.FirstRow().Cell(4).Value = "Subscribers";
                ws.FirstRow().Cell(5).Value = "IsActive";

                int row = 2;
                foreach (var channel in channelList)
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
                
                _logger.LogInformation("XLSX export completed successfully");
                
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during XLSX export");
                throw;
            }
        }
    }
}
