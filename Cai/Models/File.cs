using FluentFTP;
using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Models;

public abstract class File
{
    protected File(string name, PackIconMaterialDesignKind icon)
    {
        Name = name;
        Icon = icon;
    }

    public string Name { get; }
    public PackIconMaterialDesignKind Icon { get; }

    public abstract IEnumerable<FileData> GetFileData();
}

public class LocalFile : File
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

    public override IEnumerable<FileData> GetFileData()
    {
        switch (Item)
        {
            case DirectoryInfo directory:
                var files = directory.GetFiles("*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var length = file.FullName.Length - Item.FullName.Length;
                    var path = file.FullName.Substring(Item.FullName.Length, length);
                    var filePath = Path.Combine(Item.Name, path.TrimStart('\\').TrimStart('/'));

                    yield return new(filePath, file.OpenRead());
                }

                break;
            case FileInfo file:
                yield return new(file.Name, file.OpenRead());

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Item));
        }
    }
}

public class FtpFile : File
{
    private readonly FtpClient _ftpClient;

    public FtpFile(
        string name,
        PackIconMaterialDesignKind icon,
        FtpListItem item,
        FtpClient ftpClient
    )
        : base(name, icon)
    {
        Item = item;
        _ftpClient = ftpClient;
    }

    public FtpFile(FtpListItem item, FtpClient ftpClient)
        : base(
            item.Name,
            item.Type == FtpObjectType.Directory
                ? PackIconMaterialDesignKind.Folder
                : PackIconMaterialDesignKind.InsertDriveFile
        )
    {
        Item = item;
        _ftpClient = ftpClient;
    }

    public FtpListItem Item { get; }

    public override IEnumerable<FileData> GetFileData()
    {
        switch (Item.Type)
        {
            case FtpObjectType.Directory:
            {
                var files = _ftpClient.GetListing(
                    Item.FullName,
                    FtpListOption.Recursive | FtpListOption.AllFiles
                );

                foreach (var file in files)
                {
                    if (file.Type != FtpObjectType.File)
                    {
                        continue;
                    }

                    var length = file.FullName.Length - Item.FullName.Length;
                    var path = file.FullName.Substring(Item.FullName.Length, length);
                    var stream = new MemoryStream();
                    _ftpClient.DownloadStream(stream, file.FullName);
                    var filePath = Path.Combine(Item.Name, path.TrimStart('\\').TrimStart('/'));

                    yield return new(filePath, stream);
                }

                break;
            }
            case FtpObjectType.File:
            {
                var stream = new MemoryStream();
                _ftpClient.DownloadStream(stream, Item.FullName);

                yield return new(Item.Name, stream);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(Item));
        }
    }
}
