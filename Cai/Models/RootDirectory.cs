using FluentFTP;
using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Models;

public class DriveRootDirectory
{
    public DriveRootDirectory(string name, DriveInfo drive)
    {
        Name = name;
        Drive = drive;
        Icon = PackIconMaterialDesignKind.Folder;
    }

    public string Name { get; }
    public DriveInfo Drive { get; }
    public PackIconMaterialDesignKind Icon { get; }
}

public class RootDirectory
{
    public RootDirectory(string name, DirectoryInfo directory)
    {
        Name = name;
        Directory = directory;
        Icon = PackIconMaterialDesignKind.Folder;
    }

    public string Name { get; }
    public DirectoryInfo Directory { get; }
    public PackIconMaterialDesignKind Icon { get; }
}

public class FtpRootDirectory
{
    public FtpRootDirectory(string name, string host, string login, string password)
    {
        Name = name;
        Host = host;
        Login = login;
        Password = password;
        Icon = PackIconMaterialDesignKind.Cloud;
    }

    public string Name { get; }
    public string Host { get; }
    public string Login { get; }
    public string Password { get; }
    public PackIconMaterialDesignKind Icon { get; }

    public FtpClient CreateFtpClient()
    {
        return new(Host, Login, Password);
    }
}
