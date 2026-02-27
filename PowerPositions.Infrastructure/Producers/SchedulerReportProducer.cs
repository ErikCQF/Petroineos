using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPositions.Infrastructure.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure.Producers
{
    public class SchedulerReportProducer : BackgroundService
    {
        private readonly ILogger<SchedulerReportProducer> _logger;
        private readonly IJobQueue _queue;
        private readonly IOptions<SchedulerOptions> _options;

        public SchedulerReportProducer(
            ILogger<SchedulerReportProducer> logger,
            IJobQueue queue,
            IOptions<SchedulerOptions> options)
        {
            _logger = logger;
            _queue = queue;
            _options = options ?? throw new ArgumentNullException(nameof(SchedulerOptions));

            if (options.Value.IntervalInMinutes <= 0) throw new ArgumentNullException($"{options.Value.IntervalInMinutes} need be higher than zero");

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started");
      
            await _queue.EnqueueAsync(DateTime.UtcNow, stoppingToken);

            var interval = TimeSpan.FromMinutes(_options.Value.IntervalInMinutes);

            using var timer = new PeriodicTimer(interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await _queue.EnqueueAsync(DateTime.UtcNow, stoppingToken);
            }
        }
    }
}
