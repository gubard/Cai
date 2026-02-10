using Cai.Models;
using Gaia.Helpers;
using Gaia.Models;
using IconPacks.Avalonia.MaterialDesign;

namespace Cai.Helpers;

public static class DriveHelper
{
    public static readonly ReadOnlyMemory<FileNotify> Drives;

    static DriveHelper()
    {
        switch (OsHelper.OsType)
        {
            case Os.Linux:
                Drives = new FileNotify[]
                {
                    new("78AE0220-8BAE-4B92-BA77-2FF48CD9762B".ToGuid())
                    {
                        Name = "/",
                        Path = "/",
                        IsFrozen = true,
                        Icon = PackIconMaterialDesignKind.Folder,
                    },
                    new("70C36E25-7F0B-4686-B0F0-4E91933D1CC6".ToGuid())
                    {
                        Name = Environment.UserName,
                        Path = $"/home/{Environment.UserName}",
                        IsFrozen = true,
                        Icon = PackIconMaterialDesignKind.Folder,
                    },
                };

                break;
            default:
                Drives = DriveInfo.GetDrives().Select(x => new FileNotify(x)).ToArray();

                break;
        }
    }
}
