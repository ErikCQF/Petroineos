using Microsoft.Extensions.Logging;
using PowerPositions.Infrastructure.Entities;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure.PowerServices
{

    public class PowerDailyService : IPowerDailyService
    {
        private readonly ILogger<PowerDailyService> _logger;
        private readonly IPowerService _powerService;
        private readonly IPowerVolumeAggregator _volumeAggregator;

        public PowerDailyService(ILogger<PowerDailyService> logger,
                                 IPowerService powerService,
                                 IPowerVolumeAggregator volumeAggregator
                                 )
        {
            this._logger = logger;
            this._powerService = powerService;
            this._volumeAggregator = volumeAggregator;
        }

        public DailyPowerVolume GetDailyPowerVolume(DateTime date)
        {            
            return Consolidate(_powerService.GetTrades(date)).First() ?? new DailyPowerVolume() { ReportDate = date } ;
        }

        public async Task<DailyPowerVolume> GetDailyPowerVolumeAsync(DateTime date)
        {
            var trades = await _powerService.GetTradesAsync(date);
            return _volumeAggregator.Consolidate(trades).First() ?? new DailyPowerVolume() { ReportDate = date };
        }

        private IEnumerable<DailyPowerVolume> Consolidate(IEnumerable<PowerTrade> trades)
        {
           return _volumeAggregator.Consolidate(trades);
        }      
    }
}
