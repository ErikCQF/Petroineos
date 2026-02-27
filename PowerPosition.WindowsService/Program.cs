using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PowerPositions.Infrastructure;
using PowerPositions.Infrastructure.PowerServices;
using Serilog;

namespace PowerPosition.WindowsService;

internal class Program
{    
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)

        .UseWindowsService()

        .UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext();
        })

        .ConfigureServices((context, services) =>
        {
            services.AddPowerInfrastructure(context.Configuration);
        });

        var host = builder.Build();

        try
        {
            Log.Information("Starting Windows Service...");
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Service terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

}
