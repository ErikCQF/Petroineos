using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPositions.Infrastructure.Entities;
using System.Globalization;
using System.Text;

namespace PowerPositions.Infrastructure.Helpers
{
    public class FileDataPublisher : IDataPublisher
    {
        private readonly ILogger<FileDataPublisher> _logger;
        private readonly IFileWriter _fileWriter;
        private readonly DataPublisherOptions _settings;
        private readonly string _header = "Local;Time Volume";
        public FileDataPublisher(ILogger<FileDataPublisher> logger,
                            IOptions<DataPublisherOptions> options,
                            IFileWriter fileWriter)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));

            if (string.IsNullOrWhiteSpace(_settings?.FilePath))
                throw new ArgumentNullException("FilePath is not configured.");

            _header = string.Concat("Local Time", _settings.CsvSeparetor, "Volume"); ;
        }
        public async Task PublishAsync(DailyPowerVolume data)
        {
            string formattedDate = data.ReportDate.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture);

            var fileName = $"{_settings.StartFileName}{formattedDate}.csv";

            var filePath = Path.Combine(_settings.FilePath, fileName);

            var content = BuildCsvContent(data);

            await _fileWriter.WriteAsync(filePath, content);
        }
        private string BuildCsvContent(DailyPowerVolume data)
        {
            if (data == null) return string.Empty;
            var sb = new StringBuilder();
            sb.AppendLine(_header);

            if (data.Volume != null)
            {
                for (int i = 0; i < data.Volume.Length; i++)
                {
                    // generates the format  "23:00;150"
                    var hourValue = (i + 23) % 24; // trick got from Circular Buffer. Saves allocations and cpu branches 
                    var hour = $"{hourValue:00}:00";

                    sb.Append(hour);
                    sb.Append(_settings.CsvSeparetor); // csv separator
                    sb.Append(data.Volume[i].ToString(CultureInfo.InvariantCulture));
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
