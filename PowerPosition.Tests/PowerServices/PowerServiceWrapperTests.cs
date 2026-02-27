using Microsoft.Extensions.Logging;
using Moq;
using PowerPositions.Infrastructure.PowerServices;
using Services;

namespace PowerPosition.Tests.PowerServices
{
    public class PowerServiceWrapperTests
    {
        private readonly Mock<IPowerService> _powerServiceMock;
        private readonly Mock<IPowerVolumeAggregator> _powerVolumeAggregatorMock;
        private readonly PowerDailyService _powerServiceWrapper;
        private readonly IPowerService _powerService;

        public PowerServiceWrapperTests()
        {
            _powerServiceMock = new Mock<IPowerService>();
            _powerVolumeAggregatorMock = new Mock<IPowerVolumeAggregator>();


            var logger = new Mock<ILogger<PowerDailyService>>();

            _powerServiceWrapper = new PowerDailyService(
                logger.Object,
                _powerServiceMock.Object,
                _powerVolumeAggregatorMock.Object);

            _powerService = new PowerService();
        }

        [Fact]
        public void GetTrades_Assembly_Underlying_Service()
        {
            // Arrange
            var date = DateTime.Today;

            // Act
            var result = _powerService.GetTrades(date);

            // Assert
            Assert.True(result.Count() > 0);

        }
    }
}
