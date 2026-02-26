using PowerPositions.Infrastructure.Entities;
using Services;

namespace PowerPositions.Infrastructure.PowerServices
{
    public interface IPowerDailyService
    {
        DailyPowerVolume GetDailyPowerVolume(DateTime date);

        Task<DailyPowerVolume> GetDailyPowerVolumeAsync(DateTime date);
    }
}
