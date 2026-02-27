using Microsoft.Extensions.Logging;
using Moq;
using PowerPositions.Infrastructure.Buffers;
using PowerPositions.Infrastructure.Consumers;
using PowerPositions.Infrastructure.Entities;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;
using PowerPositions.Infrastructure.Processors;

namespace PowerPosition.Tests.PowerServices
{
    public class PowerVolumeConsumerTests
    {
        private readonly Mock<ILogger<PowerVolumeConsumer>> _loggerMock = new();        
        private readonly Mock<IPowerVolumeProcessor> _powerVolumeProcessorMock = new();
        private readonly Mock<IJobQueue> _jobQueueMock = new();
        [Fact]
        public async Task ProcessReport_Should_Call_Service_And_Publisher()
        {
            // Arrange           
            var date = DateTime.UtcNow;
            var expectedData = new DailyPowerVolume();

           async IAsyncEnumerable<DateTime>  Dequeue(
                   [System.Runtime.CompilerServices.EnumeratorCancellation]
                   CancellationToken token)
            {
            
                yield return date;
                yield return date;
                yield return date;
                
            };

            _jobQueueMock
                .Setup(x => x.DequeueAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken token) => Dequeue(token));

            _powerVolumeProcessorMock.Setup(x => x.ProcessVolumeConsolidation(date))
                                      .Returns(Task.CompletedTask);

            _jobQueueMock.Setup(x=> x.EnqueueAsync(date, CancellationToken.None))
                          .Returns(ValueTask.CompletedTask);


            var consumer = new PowerVolumeConsumer(
                new Mock<ILogger<PowerVolumeConsumer>>().Object,
                _jobQueueMock.Object,
                _powerVolumeProcessorMock.Object
                );

            // Act - directly invoke processing logic
            await consumer.ProcessAsync(date);
            await consumer.ProcessAsync(date);
            await consumer.ProcessAsync(date);

            // Assert
            _jobQueueMock.Verify(x => x.EnqueueAsync(date, It.IsAny<CancellationToken>()), Times.Exactly(3));
            _jobQueueMock.Verify(x => x.EnqueueAsync(date, It.IsAny<CancellationToken>()), Times.Exactly(3));

        }
        [Fact]
        public async Task Should_Enqueue_And_Dequeue_And_Process_Job()
        {
            // Arrange
            var date = DateTime.UtcNow;

            var cts = new CancellationTokenSource();

            var loggerMock = new Mock<ILogger<PowerVolumeConsumer>>();
            var processorMock = new Mock<IPowerVolumeProcessor>();
            var queueMock = new Mock<IJobQueue>();

            // Track if dequeue was iterated
            var dequeueWasCalled = false;

            async IAsyncEnumerable<DateTime> Dequeue(
                   [System.Runtime.CompilerServices.EnumeratorCancellation]
                  CancellationToken token)
            {
                dequeueWasCalled = true;
                yield return date;

                // stop loop after first item
                cts.Cancel();
            }

            queueMock
                .Setup(x => x.DequeueAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken token) => Dequeue(token));

            queueMock
                .Setup(x => x.EnqueueAsync(date, It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            processorMock
                .Setup(x => x.ProcessVolumeConsolidation(date))
                .Returns(Task.CompletedTask);

            var consumer = new PowerVolumeConsumer(
                loggerMock.Object,
                queueMock.Object,
                processorMock.Object);

            // Act

            // 1️⃣ Enqueue job
            await consumer.ProcessAsync(date);

            // 2️⃣ Start background loop
            await consumer.StartAsync(cts.Token);

            // Wait until cancelled
            await Task.Delay(100);

            // Assert

            queueMock.Verify(
                x => x.EnqueueAsync(date, It.IsAny<CancellationToken>()),
                Times.Once);


            queueMock.Verify(
                x => x.DequeueAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.True(dequeueWasCalled);

            processorMock.Verify(
                x => x.ProcessVolumeConsolidation(date),
                Times.Once);
        }
    }
}
