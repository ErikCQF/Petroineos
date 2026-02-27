using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPositions.Infrastructure.Buffers;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;
using PowerPositions.Infrastructure.Processors;
using System.ComponentModel;
using System.Threading.Channels;

namespace PowerPositions.Infrastructure.Consumers
{
    public class PowerVolumeConsumer : BackgroundService, IPowerVolumeConsumer
    {
        private readonly ILogger<PowerVolumeConsumer> _logger;
        private readonly IJobQueue _queue;
        private readonly IPowerVolumeProcessor _powerVolumeProcessor;
        public PowerVolumeConsumer(
            ILogger<PowerVolumeConsumer> logger,
            IJobQueue queue,
            IPowerVolumeProcessor powerVolumeProcessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _powerVolumeProcessor = powerVolumeProcessor ?? throw new ArgumentNullException(nameof(powerVolumeProcessor));
        }

        public async ValueTask ProcessAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            await _queue.EnqueueAsync(date, cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started.");

            await foreach (var date in _queue.DequeueAsync(stoppingToken))
            {
                try
                {
                    await _powerVolumeProcessor.ProcessVolumeConsolidation(date);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing report for {Date}", date);
                    throw;
                }
            }

            _logger.LogInformation("PowerVolumeWorker stopped.");
        }
    }
}
