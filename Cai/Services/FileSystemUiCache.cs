using System.Runtime.CompilerServices;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Threading;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Cai.Helpers;
using Cai.Models;
using Gaia.Helpers;
using Gaia.Services;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Helpers;
using Inanna.Services;
using File = Aya.Contract.Models.File;

namespace Cai.Services;

public interface IFileSystemUiCache
    : IUiCache<AyaPostRequest, AyaGetResponse, IFileSystemMemoryCache>
{
    IEnumerable<FileNotify> Roots { get; }
}

public interface IFileSystemMemoryCache : IMemoryCache<AyaPostRequest, AyaGetResponse>
{
    IEnumerable<FileNotify> Roots { get; }
}

public class FileSystemMemoryCache
    : MemoryCache<FileNotify, AyaPostRequest, AyaGetResponse>,
        IFileSystemMemoryCache
{
    private readonly AvaloniaList<FileNotify> _roots;

    public FileSystemMemoryCache()
    {
        _roots = new(DriveHelper.Drives.ToArray());
    }

    public IEnumerable<FileNotify> Roots => _roots;

    public override ConfiguredValueTaskAwaitable UpdateAsync(
        AyaGetResponse source,
        CancellationToken ct
    )
    {
        Update(source);

        return TaskHelper.ConfiguredCompletedTask;
    }

    private void Update(AyaGetResponse source)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
            _roots.UpdateOrder(
                DriveHelper.Drives.ToArray().Concat(source.Files.Select(Update)).ToArray()
            )
        );
    }

    public override ConfiguredValueTaskAwaitable UpdateAsync(
        AyaPostRequest source,
        CancellationToken ct
    )
    {
        Update(source);

        return TaskHelper.ConfiguredCompletedTask;
    }

    private void Update(AyaPostRequest source)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _roots.AddRange(source.CreateFiles.Select(Update).ToArray());
            _roots.RemoveAll(_roots.Where(x => source.DeleteIds.Contains(x.Id)));
        });
    }

    private FileNotify Update(File file)
    {
        var item = GetItem(file.Id);
        item.Name = file.Name;
        item.Path = file.Path;
        item.Type = file.Type;
        item.Host = file.Host;
        item.Login = file.Login;
        item.Password = file.Password;
        item.Color = Color.TryParse(file.Color, out var color) ? color : Colors.Transparent;

        item.Icon = Enum.TryParse<PackIconMaterialDesignKind>(file.Icon, out var icon)
            ? icon
            : PackIconMaterialDesignKind.Folder;

        return item;
    }
}

public sealed class FileSystemSystemUiCache
    : UiCache<AyaPostRequest, AyaGetResponse, IFileSystemDbCache, IFileSystemMemoryCache>,
        IFileSystemUiCache
{
    public FileSystemSystemUiCache(IFileSystemDbCache dbCache, IFileSystemMemoryCache memoryCache)
        : base(dbCache, memoryCache) { }

    public IEnumerable<FileNotify> Roots => MemoryCache.Roots;
}
