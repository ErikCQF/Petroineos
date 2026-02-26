using PowerPositions.Infrastructure.Entities;
using Services;
using System.Collections.Generic;

namespace PowerPositions.Infrastructure.PowerServices
{
    public interface IPowerVolumeAggregator
    {
        IEnumerable<DailyPowerVolume> Consolidate(IEnumerable<PowerTrade>? trades);
    }
}
