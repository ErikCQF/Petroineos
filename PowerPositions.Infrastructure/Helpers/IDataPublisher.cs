using PowerPositions.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure.Helpers
{
    public interface IDataPublisher
    {
        public Task PublishAsync(DailyPowerVolume data);
    }
}
