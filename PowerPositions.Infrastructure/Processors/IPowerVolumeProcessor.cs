namespace PowerPositions.Infrastructure.Processors
{
    public interface IPowerVolumeProcessor
    {
        Task ProcessVolumeConsolidation(DateTime tradeDate);
    }
}
