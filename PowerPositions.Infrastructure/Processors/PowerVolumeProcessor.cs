using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure.Processors
{
    public class PowerVolumeProcessor : IPowerVolumeProcessor
    {
        private readonly ILogger<PowerVolumeProcessor> _logger;
        private readonly IPowerDailyService _powerDailyService;
        private readonly IDataPublisher _dataPublisher;
        private readonly ResilienceOptions _resilience;

        public PowerVolumeProcessor(ILogger<PowerVolumeProcessor> logger,
                                    IOptions<ResilienceOptions> options,
                                    IPowerDailyService powerDailyService,
                                    IDataPublisher dataPublisher
                                    )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _resilience = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _powerDailyService = powerDailyService ?? throw new ArgumentNullException(nameof(powerDailyService));
            _dataPublisher = dataPublisher ?? throw new ArgumentNullException(nameof(dataPublisher));
        }

        public async Task ProcessVolumeConsolidation(DateTime tradeDate)
        {
            var attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;

                    var data = await _powerDailyService.GetDailyPowerVolumeAsync(tradeDate);
                    await _dataPublisher.PublishAsync(data);

                    return; // success
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,"Attempt {Attempt} failed for trade date {TradeDate}",attempt,tradeDate);

                    if (attempt >= _resilience.MaxRetryAttempts)
                    {
                        _logger.LogError( "Max retry attempts reached for trade date {TradeDate}", tradeDate);
                        throw;
                    }

                    await Task.Delay(_resilience.DelayMilliseconds);
                }
            }
        }
    }
}