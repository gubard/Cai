using System.Runtime.CompilerServices;
using Cai.Models;
using Inanna.Services;

namespace Cai.Services;

public interface IFilesView : IDisposable, IInitUi
{
    ConfiguredValueTaskAwaitable SaveFilesAsync(IEnumerable<FileData> files, CancellationToken ct);
}
