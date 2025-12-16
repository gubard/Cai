using Cai.Models;

namespace Cai.Services;

public interface IFilesView : IDisposable
{
    ValueTask SaveFilesAsync(IEnumerable<FileData> files, CancellationToken ct);
}
