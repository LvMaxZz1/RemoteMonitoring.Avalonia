using System.Runtime.InteropServices;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoring.Core.Services.Networks.Base;

public class SendInputHelper
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SendInput(
        uint nInputs,
        INPUT[] pInputs,
        int cbSize
    );
    
    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
    
    // 常量定义
    private const int INPUT_MOUSE = 0;
    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
    private const int SM_CXSCREEN = 0; // 屏幕宽度
    private const int SM_CYSCREEN = 1; // 屏幕高度
    
    // /// <summary>
    // /// 移动鼠标到绝对坐标或相对位置
    // /// </summary>
    // /// <param name="x">目标X坐标</param>
    // /// <param name="y">目标Y坐标</param>
    // /// <param name="isAbsolute">是否为绝对坐标（默认是）</param>
    // public static void MoveMouse(int x, int y, bool isAbsolute = true)
    // {
    //     INPUT[] inputs = new INPUT[1];
    //     int screenWidth = GetSystemMetrics(SM_CXSCREEN);
    //     int screenHeight = GetSystemMetrics(SM_CYSCREEN);
    //
    //     // 处理绝对坐标归一化（0~65535）
    //     if (isAbsolute)
    //     {
    //         inputs[0].u.mi.dx = (int)((x / (double)screenWidth) * 65535);
    //         inputs[0].u.mi.dy = (int)((y / (double)screenHeight) * 65535);
    //         inputs[0].u.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE;
    //     }
    //     else
    //     {
    //         inputs[0].u.mi.dx = x;
    //         inputs[0].u.mi.dy = y;
    //         inputs[0].u.mi.dwFlags = MOUSEEVENTF_MOVE;
    //     }
    //
    //     inputs[0].type = INPUT_MOUSE;
    //     inputs[0].u.mi.mouseData = 0;
    //     inputs[0].u.mi.time = 0;
    //     inputs[0].u.mi.dwExtraInfo = IntPtr.Zero;
    //
    //     // 发送输入事件
    //     uint result = SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
    //     if (result == 0)
    //     {
    //         int error = Marshal.GetLastWin32Error();
    //         throw new Exception($"SendInput 失败，错误码：0x{error:X}");
    //     }
    // }
}