using System.Runtime.InteropServices;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoring.Core.Utils;

public class WindowsApiHelper
{
    [DllImport("user32.dll")]
    private static extern void LockWorkStation();
    
    private const string Shutdown = "shutdown -s -t 0";
    
    private const string Restart = "shutdown -r -t 0";
    
    private const string Logout = "shutdown -l";
    
    public static void ExecuteNetworkCommand(CommandType commandType)
    {
        switch (commandType)
        {
            case CommandType.Lock:
                LockWorkStation();
                break;
            case CommandType.Shutdown:
                ExecuteWindowsCommand("shutdown -s -t 0");
                break;
            case CommandType.Restart:
                ExecuteWindowsCommand("shutdown -r -t 0");
                break;
            case CommandType.Logout:
                ExecuteWindowsCommand("shutdown -l");
                break;
        }
    }
    
    private static void ExecuteWindowsCommand(string cmdStr)
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
        info.CreateNoWindow =true;//不显示黑窗口
        info.FileName = "cmd.exe";
        info.RedirectStandardError = true;
        info.RedirectStandardInput = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        var process = System.Diagnostics.Process.Start(info);
        if (process == null) return;
        process.StandardInput.WriteLine(cmdStr + "&exit");
        process.WaitForExit();
        process.Close();
    }
}