using PowerPositions.Infrastructure.Entities;
using Services;

namespace PowerPositions.Infrastructure.PowerServices
{
    public class PowerVolumeAggregator : IPowerVolumeAggregator
    {
        public IEnumerable<DailyPowerVolume> Consolidate(IEnumerable<PowerTrade>? trades)
        {
            if (trades == null)
                return Enumerable.Empty<DailyPowerVolume>();

            var dic = new Dictionary<DateTime, DailyPowerVolume>();

            foreach (var trade in trades)
            {
                if (trade?.Periods == null)
                    continue;


                if (!dic.TryGetValue(trade.Date, out var daily))
                {
                    daily = new DailyPowerVolume
                    {
                        ReportDate = trade.Date,
                        Volume = new double [24]
                    };

                    dic[trade.Date] = daily;
                }

                var periods = trade.Periods;

                for (int i = 0; i < periods.Length; i++)
                {
                    daily.Volume[periods[i].Period -1] += periods[i].Volume;
                }
            }

            return dic.Values;

        }
    }
}
