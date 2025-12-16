using FluentFTP;
using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Models;

public class File
{
    public File(string name, PackIconMaterialDesignKind icon, FileSystemInfo item)
    {
        Name = name;
        Item = item;
        Icon = icon;
    }

    public File(FileSystemInfo item)
    {
        Name = item.Name;
        Item = item;

        Icon = Item.Attributes.HasFlag(FileAttributes.Directory)
            ? PackIconMaterialDesignKind.Folder
            : PackIconMaterialDesignKind.InsertDriveFile;
    }

    public string Name { get; }
    public FileSystemInfo Item { get; }
    public PackIconMaterialDesignKind Icon { get; }
}

public class FtpFile
{
    public FtpFile(string name, PackIconMaterialDesignKind icon, FtpListItem item)
    {
        Name = name;
        Item = item;
        Icon = icon;
    }

    public FtpFile(FtpListItem item)
    {
        Name = item.Name;
        Item = item;

        Icon = Icon =
            item.Type == FtpObjectType.Directory
                ? PackIconMaterialDesignKind.Folder
                : PackIconMaterialDesignKind.InsertDriveFile;
    }

    public string Name { get; }
    public FtpListItem Item { get; }
    public PackIconMaterialDesignKind Icon { get; }
}
