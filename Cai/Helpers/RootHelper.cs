using Cai.Models;

namespace Cai.Helpers;

public static class RootHelper
{
    public static readonly ReadOnlyMemory<RootDirectory> Roots;

    static RootHelper()
    {
        Roots = DriveInfo.GetDrives().Select(x => new RootDirectory(x.Name, new(x.Name))).ToArray();
    }
}
