using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPositions.Infrastructure.Buffers;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;
using System.ComponentModel;
using System.Threading.Channels;

namespace PowerPositions.Infrastructure.Consumers
{
    public class PowerVolumeConsumer : BackgroundService, IPowerVolumeConsumer
    {
        private readonly ILogger<PowerVolumeConsumer> _logger;
        private readonly IJobQueue _queue;
        private readonly IPowerDailyService _powerDailyService;
        private readonly IDataPublisher _dataPublisher;

        public PowerVolumeConsumer(
            ILogger<PowerVolumeConsumer> logger,
            IJobQueue queue,
            IPowerDailyService powerDailyService,
            IDataPublisher dataPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _powerDailyService = powerDailyService ?? throw new ArgumentNullException(nameof(powerDailyService));
            _dataPublisher = dataPublisher ?? throw new ArgumentNullException(nameof(dataPublisher));
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
                    var data = await _powerDailyService.GetDailyPowerVolumeAsync(date);
                    await _dataPublisher.PublishAsync(data);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing report for {Date}", date);
                }
            }

            _logger.LogInformation("PowerVolumeWorker stopped.");
        }
    }

}
