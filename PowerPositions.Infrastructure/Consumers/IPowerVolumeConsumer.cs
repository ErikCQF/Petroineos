namespace PowerPositions.Infrastructure.Consumers
{
    public interface IPowerVolumeConsumer
    {
        ValueTask ProcessAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}