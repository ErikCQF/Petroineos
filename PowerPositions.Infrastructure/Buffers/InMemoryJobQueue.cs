using System.Threading.Channels;

namespace PowerPositions.Infrastructure.Buffers
{
    public class InMemoryJobQueue : IJobQueue
    {
        private readonly Channel<DateTime> _channel;

        public InMemoryJobQueue()
        {
            var options = new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _channel = Channel.CreateBounded<DateTime>(options);
        }

        public async ValueTask EnqueueAsync(DateTime date, CancellationToken token = default)
        {
            await _channel.Writer.WriteAsync(date, token);
        }

        public IAsyncEnumerable<DateTime> DequeueAsync(CancellationToken token)
        {
            return _channel.Reader.ReadAllAsync(token);
        }
    }
}
