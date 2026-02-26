using Microsoft.Extensions.Logging;
using Moq;
using PowerPositions.Infrastructure.Buffers;
using PowerPositions.Infrastructure.Consumers;
using PowerPositions.Infrastructure.Entities;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPosition.Tests.PowerServices
{
    public class PowerVolumeWorkerTests
    {
        private readonly Mock<ILogger<PowerVolumeConsumer>> _loggerMock = new();
        private readonly Mock<IPowerDailyService> _dailyServiceMock = new();
        private readonly Mock<IDataPublisher> _publisherMock = new();
        private readonly Mock<IJobQueue> _jobQueue = new();


        private PowerVolumeConsumer CreateWorker()
        {
            return new PowerVolumeConsumer(
                _loggerMock.Object,
                _jobQueue.Object,
                _dailyServiceMock.Object,
                _publisherMock.Object);
        }

        [Fact]
        public async Task Execute_Should_Process_Queued_Date()
        {
            // Arrange
            var worker = CreateWorker();
            var date = DateTime.UtcNow;
            var expectedData = new DailyPowerVolume();

            _dailyServiceMock
                .Setup(x => x.GetDailyPowerVolumeAsync(date))
                .ReturnsAsync(expectedData);

            _publisherMock
                .Setup(x => x.PublishAsync(expectedData))
                .Returns(Task.CompletedTask);

            // Act
           // await worker.StartAsync(CancellationToken.None);
            await worker.ProcessAsync(date);

            // Give background loop time to process
            await Task.Delay(200);

            //await worker.StopAsync(CancellationToken.None);

            // Assert
            _dailyServiceMock.Verify(
                x => x.GetDailyPowerVolumeAsync(date),
                Times.Once);

            _publisherMock.Verify(
                x => x.PublishAsync(expectedData),
                Times.Once);
        }

        [Fact]
        public async Task Execute_Should_Process_Multiple_Dates()
        {
            var worker = CreateWorker();
            var date1 = DateTime.UtcNow;
            var date2 = date1.AddDays(1);

            _dailyServiceMock
                .Setup(x => x.GetDailyPowerVolumeAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new DailyPowerVolume());

            _publisherMock
                .Setup(x => x.PublishAsync(It.IsAny<DailyPowerVolume>()))
                .Returns(Task.CompletedTask);

           // await worker.StartAsync(CancellationToken.None);

            await worker.ProcessAsync(date1);
            await worker.ProcessAsync(date2);

            await Task.Delay(300);

            //await worker.StopAsync(CancellationToken.None);

            _dailyServiceMock.Verify(
                x => x.GetDailyPowerVolumeAsync(It.IsAny<DateTime>()),
                Times.Exactly(2));
        }
    }
}
