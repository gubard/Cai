using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Models;

public abstract class RootDirectory
{
    protected RootDirectory(string name, PackIconMaterialDesignKind icon, bool isFrozen, Guid id)
    {
        Name = name;
        Icon = icon;
        IsFrozen = isFrozen;
        Id = id;
    }

    public string Name { get; }
    public PackIconMaterialDesignKind Icon { get; }
    public bool IsFrozen { get; }
    public Guid Id { get; }
}

public class DriveRootDirectory : RootDirectory
{
    public DriveRootDirectory(DriveInfo drive)
        : base(drive.Name, PackIconMaterialDesignKind.Folder, true, Guid.NewGuid())
    {
        Drive = drive;
    }

    public DriveInfo Drive { get; }
}

public class LocalRootDirectory : RootDirectory
{
    public LocalRootDirectory(Guid id, DirectoryInfo directory)
        : base(directory.Name, PackIconMaterialDesignKind.Folder, false, id)
    {
        Directory = directory;
    }

    public DirectoryInfo Directory { get; }
}

public class FtpRootDirectory : RootDirectory
{
    public FtpRootDirectory(
        Guid id,
        string name,
        string host,
        string login,
        string password,
        string path
    )
        : base(name, PackIconMaterialDesignKind.Cloud, false, id)
    {
        Host = host;
        Login = login;
        Password = password;
        Path = path;
    }

    public string Host { get; }
    public string Login { get; }
    public string Password { get; }
    public string Path { get; }
}
