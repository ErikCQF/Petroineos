using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure.Processors
{
    public class ResilienceOptions
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public int DelayMilliseconds { get; set; } = 1000;
    }

}
