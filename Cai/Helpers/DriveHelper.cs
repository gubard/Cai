using Cai.Models;

namespace Cai.Helpers;

public static class DriveHelper
{
    public static readonly ReadOnlyMemory<FileNotify> Drives;

    static DriveHelper()
    {
        Drives = DriveInfo.GetDrives().Select(x => new FileNotify(x)).ToArray();
    }
}
