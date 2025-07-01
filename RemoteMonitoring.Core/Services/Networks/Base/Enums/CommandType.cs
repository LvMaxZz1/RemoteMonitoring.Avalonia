using System.ComponentModel;

namespace RemoteMonitoring.Core.Services.Networks.Base.Enums;

public enum CommandType : byte
{
    [Description("发送通知")]
    SendNotice = 0x0,
    
    [Description("锁定")]
    Lock = 0x01,
    
    [Description("关机")]
    Shutdown = 0x02,
    
    [Description("重启")]
    Restart = 0x03,
    
    [Description("注销")]
    Logout = 0x04,
    
    [Description("捕获屏幕")]
    ObtainScreen = 0x05,
    
    [Description("获取文件")]
    ObtainFiles = 0x06,
    
    [Description("传输文件")]
    TransferFiles = 0x07,
    
    [Description("发送终端命令")]
    SendTerminalCommand = 0x08,
    
    [Description("基础控制保留位2")]
    ReservedControl2 = 0x09,
    
    [Description("关闭Avalonia应用程序")]
    AvaloniaShutdown = 0x10,
    
    [Description("高级控制保留位1")]
    ReservedAdvanced1 = 0x11
}