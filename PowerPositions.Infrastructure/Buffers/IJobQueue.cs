using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPositions.Infrastructure.Buffers
{
    public interface IJobQueue
    {
        ValueTask EnqueueAsync(DateTime date, CancellationToken token = default);
        IAsyncEnumerable<DateTime> DequeueAsync(CancellationToken token);
    }
}
