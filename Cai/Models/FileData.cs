namespace Cai.Models;

public sealed class FileData : IDisposable, IAsyncDisposable
{
    public FileData(string path, Stream stream)
    {
        Path = path;
        Stream = stream;
    }

    public string Path { get; }
    public Stream Stream { get; }

    public void Dispose()
    {
        Stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();
    }
}
