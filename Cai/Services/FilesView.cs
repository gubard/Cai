using System.Runtime.CompilerServices;
using Cai.Models;

namespace Cai.Services;

public interface IFilesView : IDisposable
{
    ConfiguredValueTaskAwaitable SaveFilesAsync(IEnumerable<FileData> files, CancellationToken ct);
}
