
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerPositions.Infrastructure.Buffers;
using PowerPositions.Infrastructure.Consumers;
using PowerPositions.Infrastructure.Helpers;
using PowerPositions.Infrastructure.PowerServices;
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


            // Services
            services.AddScoped<IDataPublisher, FileDataPublisher>();
            services.AddScoped<IFileWriter, FileWriter>();
            services.AddScoped<IPowerService, PowerService>();
            services.AddScoped<IPowerVolumeAggregator, PowerVolumeAggregator>();
            services.AddScoped<IPowerDailyService, PowerDailyService>();

            services.AddSingleton<IJobQueue, InMemoryJobQueue>();

            
            

            // Worker / Background processing
            services.AddHostedService<PowerVolumeConsumer>();
            services.AddHostedService<SchedulerReportProducer>();


            return services;
        
        }
    }
}
