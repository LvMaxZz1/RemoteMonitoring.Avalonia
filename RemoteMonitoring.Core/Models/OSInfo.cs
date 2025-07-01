namespace RemoteMonitoring.Core.Models;

public class OSInfo
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Build { get; set; }
    public int IS64Bit { get; set; }

    public OSInfo()
    {
        Major = Environment.OSVersion.Version.Major;
        Minor = Environment.OSVersion.Version.Minor;
        Build = Environment.OSVersion.Version.Build;
        IS64Bit = IntPtr.Size * 8 == 64 ? 1 : 0;
    }
}