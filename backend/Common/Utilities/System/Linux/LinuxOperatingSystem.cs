namespace FitHub.Common.Utilities.System.Linux;

public static class LinuxOperatingSystem
{
    public static CgroupVersion GetCgroupVersion()
    {
        if (!OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException("Система должна быть Linux для получения версии cgroup");
        }

        try
        {
            // /sys/fs/cgroup is the standard mount point on Linux
            var drive = new DriveInfo("/sys/fs/cgroup");

            return drive.DriveFormat switch
            {
                "cgroup2" => CgroupVersion.V2,   // .NET 10 specific string
                "cgroup2fs" => CgroupVersion.V2, // Pre-.NET 10 string (still possible on some distros)
                "tmpfs" => CgroupVersion.V1,     // v1 root is usually a tmpfs mount
                _ => CgroupVersion.Unknown
            };
        }
        catch
        {
            return CgroupVersion.NotSupported;
        }
    }
}
