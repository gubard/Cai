using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Models;

public class File
{
    public File(string name, PackIconMaterialDesignKind icon, FileSystemInfo fileInfo)
    {
        Name = name;
        FileInfo = fileInfo;
        Icon = icon;
    }

    public File(FileSystemInfo fileInfo)
    {
        Name = fileInfo.Name;
        FileInfo = fileInfo;

        Icon = FileInfo.Attributes.HasFlag(FileAttributes.Directory)
            ? PackIconMaterialDesignKind.Folder
            : PackIconMaterialDesignKind.InsertDriveFile;
    }

    public string Name { get; }
    public FileSystemInfo FileInfo { get; }
    public PackIconMaterialDesignKind Icon { get; }
}
