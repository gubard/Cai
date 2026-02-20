using System.Runtime.CompilerServices;
using Cai.Models;
using Inanna.Services;

namespace Cai.Services;

public interface IFilesView : IDisposable, IInit
{
    string BasePath { get; }

    ConfiguredValueTaskAwaitable SaveFilesAsync(
        IEnumerable<FileData> files,
        string basePath,
        CancellationToken ct
    );
}
