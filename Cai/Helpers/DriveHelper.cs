using Cai.Models;

namespace Cai.Helpers;

public static class DriveHelper
{
    public static readonly ReadOnlyMemory<DriveRootDirectory> Roots;

    static DriveHelper()
    {
        Roots = DriveInfo.GetDrives().Select(x => new DriveRootDirectory(x)).ToArray();
    }
}
