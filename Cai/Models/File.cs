using System.Runtime.CompilerServices;
using Avalonia.Media;
using Aya.Contract.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Models;

public sealed partial class FileNotify : ObservableObject, IStaticFactory<Guid, FileNotify>
{
    public FileNotify(Guid id)
    {
        Id = id;
    }

    public FileNotify(DriveInfo drive)
    {
        Path = drive.RootDirectory.FullName;
        Name = drive.Name;
        Icon = PackIconMaterialDesignKind.Folder;
        Id = Guid.NewGuid();
        IsFrozen = true;
    }

    public Guid Id { get; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial FileType Type { get; set; }

    [ObservableProperty]
    public partial PackIconMaterialDesignKind Icon { get; set; }

    [ObservableProperty]
    public partial Color Color { get; set; }

    [ObservableProperty]
    public partial string Path { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Login { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Host { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsFrozen { get; set; }

    public static FileNotify Create(Guid input)
    {
        return new(input);
    }
}

public abstract class File
{
    protected File(string name, PackIconMaterialDesignKind icon)
    {
        Name = name;
        Icon = icon;
    }

    public string Name { get; }
    public PackIconMaterialDesignKind Icon { get; }

    public abstract ConfiguredValueTaskAwaitable<IEnumerable<FileData>> GetFileDataAsync(
        CancellationToken ct
    );
}

public sealed class LocalFile : File
{
    public LocalFile(string name, PackIconMaterialDesignKind icon, FileSystemInfo item)
        : base(name, icon)
    {
        Item = item;
    }

    public LocalFile(FileSystemInfo item)
        : base(
            item.Name,
            item.Attributes.HasFlag(FileAttributes.Directory)
                ? PackIconMaterialDesignKind.Folder
                : PackIconMaterialDesignKind.InsertDriveFile
        )
    {
        Item = item;
    }

    public FileSystemInfo Item { get; }

    public override ConfiguredValueTaskAwaitable<IEnumerable<FileData>> GetFileDataAsync(
        CancellationToken ct
    )
    {
        var result = new List<FileData>();

        switch (Item)
        {
            case DirectoryInfo directory:
                var files = directory.GetFiles("*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var length = file.FullName.Length - Item.FullName.Length;
                    var path = file.FullName.Substring(Item.FullName.Length, length);
                    var filePath = Path.Combine(Item.Name, path.TrimStart('\\').TrimStart('/'));

                    result.Add(new(filePath, file.OpenRead()));
                }

                break;
            case FileInfo file:
                result.Add(new(file.Name, file.OpenRead()));

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Item));
        }

        return TaskHelper.FromResult(result.AsEnumerable());
    }
}

public sealed class FtpFile : File
{
    private readonly IFtpClientService _ftpClient;

    public FtpFile(
        string name,
        PackIconMaterialDesignKind icon,
        FtpItem item,
        IFtpClientService ftpClient
    )
        : base(name, icon)
    {
        Item = item;
        _ftpClient = ftpClient;
    }

    public FtpFile(FtpItem item, IFtpClientService ftpClient)
        : base(
            Path.GetFileName(item.Path),
            item.Type == FtpItemType.Directory
                ? PackIconMaterialDesignKind.Folder
                : PackIconMaterialDesignKind.InsertDriveFile
        )
    {
        Item = item;
        _ftpClient = ftpClient;
    }

    public FtpItem Item { get; }

    public override ConfiguredValueTaskAwaitable<IEnumerable<FileData>> GetFileDataAsync(
        CancellationToken ct
    )
    {
        return GetFileDataCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<IEnumerable<FileData>> GetFileDataCore(CancellationToken ct)
    {
        var result = new List<FileData>();

        switch (Item.Type)
        {
            case FtpItemType.Directory:
            {
                var files = await _ftpClient.GetListItemAsync(Item.Path, ct);
                var filesArray = files.ToArray();

                foreach (var file in filesArray)
                {
                    if (file.Type != FtpItemType.File)
                    {
                        continue;
                    }

                    var length = file.Path.Length - Item.Path.Length;
                    var path = file.Path.Substring(Item.Path.Length, length);
                    var stream = new MemoryStream();
                    await _ftpClient.DownloadItemAsync(file.Path, stream, ct);
                    stream.Position = 0;

                    var filePath = Path.Combine(
                        Path.GetFileName(Item.Path),
                        path.TrimStart('\\').TrimStart('/')
                    );

                    result.Add(new(filePath, stream));
                }

                break;
            }
            case FtpItemType.File:
            {
                var stream = new MemoryStream();
                await _ftpClient.DownloadItemAsync(Item.Path, stream, ct);
                stream.Position = 0;
                result.Add(new(Item.Path, stream));

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(Item));
        }

        return result;
    }
}
