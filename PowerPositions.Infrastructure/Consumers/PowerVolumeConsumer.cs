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


    public class PowerVolumeWorker_BackUp : BackgroundService//, IPowerVolumeConsumer
    {
        private readonly ILogger<PowerVolumeConsumer> _logger;
        private readonly IPowerDailyService _powerDailyService;
        private readonly IDataPublisher _dataPublisher;
        private readonly Channel<DateTime> _channel;
        public PowerVolumeWorker_BackUp(ILogger<PowerVolumeConsumer> logger,
                             IPowerDailyService powerDailyService,
                             IDataPublisher dataPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _powerDailyService = powerDailyService ?? throw new ArgumentNullException(nameof(powerDailyService));
            _dataPublisher = dataPublisher ?? throw new ArgumentNullException(nameof(dataPublisher));
            _channel = CreateChannel();
        }

        public async ValueTask ProcessAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            await _channel.Writer.WriteAsync(date, cancellationToken);
        }
        private Channel<DateTime> CreateChannel()
        {
            var options = new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            var channel = Channel.CreateBounded<DateTime>(options);

            return channel;
        }
        private async Task ProcessReport(DateTime evt)
        {
            try
            {
                var data = await _powerDailyService.GetDailyPowerVolumeAsync(evt);

                await _dataPublisher.PublishAsync(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing report for {Date}", evt);
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping PowerVolumeWorker...");

            _channel.Writer.TryComplete();

            await base.StopAsync(cancellationToken);
        }
        public override void Dispose()
        {
            _channel.Writer.TryComplete();
            base.Dispose();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PowerVolumeWorker started.");

            try
            {
                await foreach (var date in _channel.Reader.ReadAllAsync(stoppingToken))
                {
                    await ProcessReport(date);
                }
            }

            catch (OperationCanceledException)
            {
                _logger.LogInformation("PowerVolumeWorker stopping due to cancellation.");
            }

            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled error in worker loop.");
            }

            _logger.LogInformation("PowerVolumeWorker stopped.");
        }
    }
}
