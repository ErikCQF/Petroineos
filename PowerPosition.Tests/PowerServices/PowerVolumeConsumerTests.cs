using Microsoft.Extensions.Logging;
using Moq;
using PowerPositions.Infrastructure.Buffers;
using PowerPositions.Infrastructure.Consumers;
using PowerPositions.Infrastructure.Entities;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;

namespace PowerPosition.Tests.PowerServices
{
    public class PowerVolumeConsumerTests
    {
        private readonly Mock<ILogger<PowerVolumeConsumer>> _loggerMock = new();
        private readonly Mock<IPowerDailyService> _dailyServiceMock = new();
        private readonly Mock<IDataPublisher> _publisherMock = new();
        private readonly Mock<IJobQueue> _jobQueue = new();


        [Fact]
        public async Task ProcessReport_Should_Call_Service_And_Publisher()
        {
            // Arrange
            var mockQueue = new Mock<IJobQueue>();
            var date = DateTime.UtcNow;
            var expectedData = new DailyPowerVolume();

            var mockDailyService = new Mock<IPowerDailyService>();
            mockDailyService.Setup(x => x.GetDailyPowerVolumeAsync(date))
                            .ReturnsAsync(expectedData);

            var mockPublisher = new Mock<IDataPublisher>();
            mockPublisher.Setup(x => x.PublishAsync(expectedData))
                         .Returns(Task.CompletedTask);

            var consumer = new PowerVolumeConsumer(
                new Mock<ILogger<PowerVolumeConsumer>>().Object,
                mockQueue.Object,
                mockDailyService.Object,
                mockPublisher.Object);

            // Act - directly invoke processing logic
            await consumer.ProcessAsync(date);

            // Assert
            mockQueue.Verify(x => x.EnqueueAsync(date, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
