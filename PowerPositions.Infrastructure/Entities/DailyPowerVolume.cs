using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure.Entities
{

    public class DailyPowerVolume
    {
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// The index represents the period. Trick way to save space
        /// </summary>
        public double[] Volume { get; set; } = new double[24];
    }

}
