using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PowerPositions.Infrastructure;
using PowerPositions.Infrastructure.PowerServices;

namespace PowerPosition.WindowsService;

internal class Program
{    
    static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((context, services) =>
            {
                services.AddPowerInfrastructure(context.Configuration);
            })
            .Build()
            .RunAsync();
    }

}
