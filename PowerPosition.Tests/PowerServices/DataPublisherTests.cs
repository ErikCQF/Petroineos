using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PowerPositions.Infrastructure.Entities;
using PowerPositions.Infrastructure.Helpers;

namespace PowerPosition.Tests.PowerServices
{
    public class DataPublisherTests
    {
        [Fact]
        public async Task PublishAsync_Should_Generate_Correct_Csv_Format()
        {
            // Arrange
            var mockFileWriter = new Mock<IFileWriter>();
            var mockLogger = new Mock<ILogger<FileDataPublisher>>();

            var options = Options.Create(new DataPublisherOptions
            {
                FilePath = "C:\\Temp",
                StartFileName = "PowerPosition",
                CsvSeparetor = ";"
            });

            var publisher = new FileDataPublisher(
                mockLogger.Object,
                options,
                mockFileWriter.Object);

            var data = new DailyPowerVolume
            {
                ReportDate = new DateTime(2026, 02, 26, 14, 0, 0),
                Volume = new double[24]
            };

            data.Volume[0] = 150;

            // Act
            await publisher.PublishAsync(data);

            // Assert
            mockFileWriter.Verify(x =>
                x.WriteAsync(
                    It.IsAny<string>(),
                    It.Is<string>(content =>
                        content.Contains("Local Time;Volume") &&
                        content.Contains("23:00;150")
                    )
                ),
                Times.Once);
        }
    }
}
