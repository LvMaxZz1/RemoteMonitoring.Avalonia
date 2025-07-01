using System.Runtime.InteropServices;

namespace RemoteMonitoring.Core.Services.Networks.Base.Enums;

[Flags]
public enum MOUSEEVENTF
{
    MOVE = 0x0001,  //mouse move     
    LEFTDOWN = 0x0002,  //left button down     
    LEFTUP = 0x0004,  //left button up     
    RIGHTDOWN = 0x0008,  //right button down     
    RIGHTUP = 0x0010,  //right button up     
    MIDDLEDOWN = 0x0020, //middle button down     
    MIDDLEUP = 0x0040,  //middle button up     
    XDOWN = 0x0080,  //x button down     
    XUP = 0x0100,  //x button down     
    WHEEL = 0x0800,  //wheel button rolled     
    VIRTUALDESK = 0x4000,  //map to entire virtual desktop     
    ABSOLUTE = 0x8000,  //absolute move     
}

[Flags()]
public enum KEYEVENTF
{
    EXTENDEDKEY = 0x0001,
    KEYUP = 0x0002,
    UNICODE = 0x0004,
    SCANCODE = 0x0008,
}

[StructLayout(LayoutKind.Explicit)]
public struct INPUT
{
    [FieldOffset(0)]
    public Int32 type;//0-MOUSEINPUT;1-KEYBDINPUT;2-HARDWAREINPUT     
    [FieldOffset(4)]
    public KEYBDINPUT ki;
    [FieldOffset(4)]
    public MOUSEINPUT mi;
    [FieldOffset(4)]
    public HARDWAREINPUT hi;
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSEINPUT
{
    public Int32 dx;
    public Int32 dy;
    public UInt32 dwData;
    public UInt32 dwFlags;
    public UInt32 time;
    public UInt32 dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
public struct KEYBDINPUT
{
    public UInt16 wVk;
    public UInt16 wScan;
    public UInt32 dwFlags;
    public UInt32 time;
    public UInt32 dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
public struct HARDWAREINPUT
{
    public UInt32 uMsg;
    public UInt16 wParamL;
    public UInt16 wParamH;
}

// [StructLayout(LayoutKind.Sequential)]
// public struct MOUSEINPUT
// {
//     public int dx;        // 坐标偏移或绝对位置
//     public int dy;
//     public uint mouseData; // 滚轮或侧键数据
//     public uint dwFlags;  // 事件标志（如MOUSEEVENTF_LEFTDOWN）
//     public uint time;
//     public IntPtr dwExtraInfo;
// }
//
// [StructLayout(LayoutKind.Sequential)]
// public struct KEYBDINPUT
// {
//     public ushort wVk;     // 虚拟键码（如0x41表示A键）
//     public ushort wScan;
//     public uint dwFlags;   // 事件标志（如KEYEVENTF_KEYUP）
//     public uint time;
//     public IntPtr dwExtraInfo;
// }
//
// [StructLayout(LayoutKind.Explicit)]
// public struct INPUTUNION
// {
//     [FieldOffset(0)] public MOUSEINPUT mi;
//     [FieldOffset(0)] public KEYBDINPUT ki;
// }
//
// [StructLayout(LayoutKind.Sequential)]
// public struct INPUT
// {
//     public uint type;      // 输入类型：1=键盘，0=鼠标
//     public INPUTUNION u;
// }
//
// [Flags]
// public enum VirtualKeyCode : ushort
// {
//     // 鼠标键（通常不用于键盘事件）
//     VK_LBUTTON    = 0x01,   // 鼠标左键[1,3](@ref)
//     VK_RBUTTON    = 0x02,   // 鼠标右键[1,3](@ref)
//     VK_MBUTTON    = 0x04,   // 鼠标中键[1,3](@ref)
//     VK_XBUTTON1   = 0x05,   // 鼠标后退键[7](@ref)
//     VK_XBUTTON2   = 0x06,   // 鼠标前进键[7](@ref)
//
//     // 控制键与导航键
//     VK_BACK       = 0x08,   // Backspace[1,3](@ref)
//     VK_TAB        = 0x09,   // Tab[1,4](@ref)
//     VK_CLEAR      = 0x0C,   // NumLock关闭时的数字5键[3](@ref)
//     VK_RETURN     = 0x0D,   // Enter[1,3](@ref)
//     VK_SHIFT      = 0x10,   // Shift[1,4](@ref)
//     VK_CONTROL    = 0x11,   // Ctrl[1,4](@ref)
//     VK_MENU       = 0x12,   // Alt[1,4](@ref)
//     VK_PAUSE      = 0x13,   // Pause[3,4](@ref)
//     VK_CAPITAL    = 0x14,   // Caps Lock[1,3](@ref)
//     VK_ESCAPE     = 0x1B,   // Esc[1,3](@ref)
//     VK_SPACE      = 0x20,   // 空格键[1,3](@ref)
//     VK_PRIOR      = 0x21,   // Page Up[1,4](@ref)
//     VK_NEXT       = 0x22,   // Page Down[1,4](@ref)
//     VK_END        = 0x23,   // End[1,4](@ref)
//     VK_HOME       = 0x24,   // Home[1,4](@ref)
//     VK_LEFT       = 0x25,   // ←[1,3](@ref)
//     VK_UP         = 0x26,   // ↑[1,3](@ref)
//     VK_RIGHT      = 0x27,   // →[1,3](@ref)
//     VK_DOWN       = 0x28,   // ↓[1,3](@ref)
//     VK_SELECT     = 0x29,   // Select[3](@ref)
//     VK_PRINT      = 0x2A,   // Print[3](@ref)
//     VK_EXECUTE    = 0x2B,   // Execute[3](@ref)
//     VK_SNAPSHOT   = 0x2C,   // Print Screen[1,3](@ref)
//     VK_INSERT     = 0x2D,   // Insert[1,3](@ref)
//     VK_DELETE     = 0x2E,   // Delete[1,3](@ref)
//     VK_HELP       = 0x2F,   // Help[3](@ref)
//
//     // 数字与字母键
//     VK_0          = 0x30,   // 0[1,3](@ref)
//     VK_1          = 0x31,   // 1[1,3](@ref)
//     // ... 其他数字键 VK_2 到 VK_9（0x32-0x39）
//     VK_A          = 0x41,   // A[1,3](@ref)
//     VK_B          = 0x42,   // B[1,3](@ref)
//     // ... 其他字母键 VK_C 到 VK_Z（0x43-0x5A）
//
//     // 功能键与系统键
//     VK_LWIN       = 0x5B,   // 左Win键[5,7](@ref)
//     VK_RWIN       = 0x5C,   // 右Win键[5,7](@ref)
//     VK_APPS       = 0x5D,   // 应用键（菜单键）[5,7](@ref)
//     VK_SLEEP      = 0x5F,   // 睡眠键[7](@ref)
//     VK_F1         = 0x70,   // F1[5,7](@ref)
//     VK_F2         = 0x71,   // F2[5,7](@ref)
//     // ... 其他功能键 VK_F3 到 VK_F24（0x72-0x87）
//
//     // 数字小键盘
//     VK_NUMPAD0    = 0x60,   // 小键盘0[1,3](@ref)
//     VK_NUMPAD1    = 0x61,   // 小键盘1[1,3](@ref)
//     // ... 其他小键盘数字键 VK_NUMPAD2 到 VK_NUMPAD9（0x62-0x69）
//     VK_MULTIPLY   = 0x6A,   // 小键盘*[1,3](@ref)
//     VK_ADD        = 0x6B,   // 小键盘+[1,3](@ref)
//     VK_SEPARATOR  = 0x6C,   // Separator[3](@ref)
//     VK_SUBTRACT   = 0x6D,   // 小键盘-[1,3](@ref)
//     VK_DECIMAL    = 0x6E,   // 小键盘.[1,3](@ref)
//     VK_DIVIDE     = 0x6F,   // 小键盘/[1,3](@ref)
//
//     // 其他特殊键
//     VK_NUMLOCK    = 0x90,   // Num Lock[3,7](@ref)
//     VK_SCROLL     = 0x91,   // Scroll Lock[3,7](@ref)
//     VK_VOLUME_MUTE= 0xAD,   // 静音键（部分键盘支持）
//     VK_VOLUME_DOWN= 0xAE,   // 音量减
//     VK_VOLUME_UP  = 0xAF    // 音量加
// }
