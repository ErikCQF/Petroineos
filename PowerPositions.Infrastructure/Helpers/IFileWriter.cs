namespace PowerPositions.Infrastructure.Helpers
{
    public interface IFileWriter
    {
        Task WriteAsync(string path, string content);
    }
}
