using Cai.Models;

namespace Cai.Helpers;

public static class RootHelper
{
    public static readonly ReadOnlyMemory<DriveRootDirectory> Roots;

    static RootHelper()
    {
        Roots = DriveInfo.GetDrives().Select(x => new DriveRootDirectory(x.Name, x)).ToArray();
    }
}
