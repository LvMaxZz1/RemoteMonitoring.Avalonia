using System;
using System.Runtime.InteropServices;

namespace RemoteMonitoringClient;

internal static class Win32Api
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
    
    [DllImport("user32.dll")]
    internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("gdi32.dll")]
    internal static extern bool BitBlt(
        IntPtr hdcDest, // 目标 DC的句柄 
        int nXDest,
        int nYDest,
        int nWidth,
        int nHeight,
        IntPtr hdcSrc, // 源DC的句柄 
        int nXSrc,
        int nYSrc,
        Int32 dwRop // 光栅的处理数值 
    ); 
    
    [DllImport("gdi32.dll")]
    internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    
    [DllImport("gdi32.dll")]
    internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);
    
    [DllImport("gdi32.dll")]
    internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
    
    [DllImport("gdi32.dll")]
    internal static extern bool GetDIBits(IntPtr hdc, IntPtr hbmp, uint start, uint lines, 
        IntPtr pixels, ref BITMAPINFO bmi, uint usage);
    
    [DllImport("gdi32.dll")]
    internal static extern bool DeleteObject(IntPtr hObject);
    
    [DllImport("user32.dll")]
    internal static extern IntPtr GetDC(IntPtr hwnd);
    
    [DllImport("gdi32.dll")]
    internal static extern bool DeleteDC(IntPtr hdc);
    
    [DllImport("user32.dll")]
    internal  static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
    
    [DllImport("user32.dll")]
    internal static extern uint GetDoubleClickTime();
    
    [DllImport("user32.dll")]
    internal static extern uint SetDoubleClickTime(int wCount);
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFO
    {
        internal BITMAPINFOHEADER bmiHeader;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }
}