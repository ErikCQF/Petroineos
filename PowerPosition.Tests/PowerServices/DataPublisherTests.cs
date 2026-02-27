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
            data.Volume[1] = 151;
            data.Volume[2] = 152;
            data.Volume[3] = 153;
            data.Volume[4] = 154;
            data.Volume[5] = 155;
            data.Volume[6] = 156;
            data.Volume[7] = 157;
            data.Volume[8] = 158;
            data.Volume[9] = 159;
            data.Volume[10] = 160;
            data.Volume[11] = 161;
            data.Volume[12] = 162;
            data.Volume[13] = 163;
            data.Volume[14] = 164;
            data.Volume[15] = 165;
            data.Volume[16] = 166;
            data.Volume[17] = 167;
            data.Volume[18] = 168;
            data.Volume[19] = 169;
            data.Volume[20] = 170;
            data.Volume[21] = 171;
            data.Volume[22] = 172;
            data.Volume[23] = -10;



            // Act
            await publisher.PublishAsync(data);

            // Assert
            mockFileWriter.Verify(x =>
                x.WriteAsync(
                    It.IsAny<string>(),
                    It.Is<string>(content =>
                        content.Contains("Local Time;Volume") &&
                        content.Contains("23:00;150") &&
                        content.Contains("00:00;151") &&
                        content.Contains("01:00;152") &&
                        content.Contains("02:00;153") &&
                        content.Contains("03:00;154") &&
                        content.Contains("04:00;155") &&
                        content.Contains("05:00;156") &&
                        content.Contains("06:00;157") &&
                        content.Contains("07:00;158") &&
                        content.Contains("08:00;159") &&
                        content.Contains("09:00;160") &&
                        content.Contains("10:00;161") &&
                        content.Contains("11:00;162") &&
                        content.Contains("12:00;163") &&
                        content.Contains("13:00;164") &&
                        content.Contains("14:00;165") &&
                        content.Contains("15:00;166") &&
                        content.Contains("16:00;167") &&
                        content.Contains("17:00;168") &&
                        content.Contains("18:00;169") &&
                        content.Contains("19:00;170") &&
                        content.Contains("20:00;171") &&
                        content.Contains("21:00;172") &&
                        content.Contains("22:00;-10") 
                    )
                ),
                Times.Once);
        }


    }
}
