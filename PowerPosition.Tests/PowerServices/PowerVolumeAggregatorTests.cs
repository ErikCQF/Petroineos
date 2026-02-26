using PowerPositions.Infrastructure.PowerServices;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPosition.Tests.PowerServices
{

    public class PowerVolumeAggregatorTests
    {
        [Fact]
        public void Consolidate_Should_Aggregate_Volumes_For_Same_Date()
        {
            // Arrange
            var date = new DateTime(2026, 01, 01);

            var trade1 = CreateTrade(date, new double[] { 10 });
            var trade2 = CreateTrade(date, new double[] { 10 });

            var trades = new List<PowerTrade>
            {
              trade1,
              trade2
            };

            var sut = new PowerVolumeAggregator();

            // Act
            var result = sut.Consolidate(trades).ToList();

            var daily = result.FirstOrDefault();

            // Assert
       
            Assert.NotEmpty(result);
            Assert.Equal(20, daily?.Volume[0]);
        }

        private PowerTrade CreateTrade(DateTime date, double[] volume)
        {
            var tr =   Services.PowerTrade.Create(date, volume.Length);
            for (var i = 0; i < volume.Length; i++) 
            {
                tr.Periods[i].Period = i;
                tr.Periods[i].Volume = volume[i];
            }

            return tr;
        }
    }
}
