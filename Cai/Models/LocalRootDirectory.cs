using FluentFTP;
using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Models;

public abstract class RootDirectory
{
    protected RootDirectory(string name, PackIconMaterialDesignKind icon)
    {
        Name = name;
        Icon = icon;
    }

    public string Name { get; }
    public PackIconMaterialDesignKind Icon { get; }
}

public class DriveRootDirectory : RootDirectory
{
    public DriveRootDirectory(DriveInfo drive)
        : base(drive.Name, PackIconMaterialDesignKind.Folder)
    {
        Drive = drive;
    }

    public DriveInfo Drive { get; }
}

public class LocalRootDirectory : RootDirectory
{
    public LocalRootDirectory(DirectoryInfo directory)
        : base(directory.Name, PackIconMaterialDesignKind.Folder)
    {
        Directory = directory;
    }

    public DirectoryInfo Directory { get; }
}

public class FtpRootDirectory : RootDirectory
{
    public FtpRootDirectory(string name, string host, string login, string password)
        : base(name, PackIconMaterialDesignKind.Cloud)
    {
        Host = host;
        Login = login;
        Password = password;
    }

    public string Host { get; }
    public string Login { get; }
    public string Password { get; }

    public FtpClient CreateFtpClient()
    {
        return new(Host, Login, Password);
    }
}
