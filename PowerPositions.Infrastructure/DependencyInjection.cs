
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerPositions.Infrastructure.Buffers;
using PowerPositions.Infrastructure.Consumers;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;
using PowerPositions.Infrastructure.Processors;
using PowerPositions.Infrastructure.Producers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPowerInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Options
            services.Configure<DataPublisherOptions>(configuration.GetSection("DataPublisher"));
            services.Configure<SchedulerOptions>(configuration.GetSection("Scheduler"));
            services.Configure<ResilienceOptions>(configuration.GetSection("Resilience"));

            // Services            
            services.AddScoped<IPowerService, PowerService>();

            services.AddSingleton<IFileWriter, FileWriter>();
            services.AddSingleton<IPowerVolumeAggregator, PowerVolumeAggregator>();            
            services.AddSingleton<IJobQueue, InMemoryJobQueue>();
            services.AddSingleton<IDataPublisher, FileDataPublisher>();
            services.AddSingleton<IPowerDailyService, PowerDailyService>();
            services.AddSingleton<IPowerVolumeProcessor, PowerVolumeProcessor>();

            // Worker / Background processing
            services.AddHostedService<PowerVolumeConsumer>();
            services.AddHostedService<SchedulerReportProducer>();

            return services;        
        }
    }
}
