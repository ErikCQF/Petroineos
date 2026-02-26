using System.Text;

namespace PowerPositions.Infrastructure.Helpers
{
    public class FileWriter : IFileWriter
    {
        public async Task WriteAsync(string path, string content)
        {
            await File.WriteAllTextAsync(path, content, Encoding.UTF8);
        }
    }
}
