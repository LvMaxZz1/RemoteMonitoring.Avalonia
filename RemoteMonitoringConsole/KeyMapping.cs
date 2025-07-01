using System;
using System.Collections.Generic;
using Avalonia.Input;

namespace RemoteMonitoringConsole;

public static class KeyMapping
{
    // Avalonia Key → Win32 虚拟键码 (VK_XXX)
    public static readonly Dictionary<Key, byte> AvaloniaToWin32VK = new Dictionary<Key, byte>
    {
        // 字母键 A-Z (0x41-0x5A)
        [Key.A] = 0x41, [Key.B] = 0x42, [Key.C] = 0x43, [Key.D] = 0x44,
        [Key.E] = 0x45, [Key.F] = 0x46, [Key.G] = 0x47, [Key.H] = 0x48,
        [Key.I] = 0x49, [Key.J] = 0x4A, [Key.K] = 0x4B, [Key.L] = 0x4C,
        [Key.M] = 0x4D, [Key.N] = 0x4E, [Key.O] = 0x4F, [Key.P] = 0x50,
        [Key.Q] = 0x51, [Key.R] = 0x52, [Key.S] = 0x53, [Key.T] = 0x54,
        [Key.U] = 0x55, [Key.V] = 0x56, [Key.W] = 0x57, [Key.X] = 0x58,
        [Key.Y] = 0x59, [Key.Z] = 0x5A,

        // 主键盘数字键 0-9 (0x30-0x39)
        [Key.D0] = 0x30, [Key.D1] = 0x31, [Key.D2] = 0x32, 
        [Key.D3] = 0x33, [Key.D4] = 0x34, [Key.D5] = 0x35,
        [Key.D6] = 0x36, [Key.D7] = 0x37, [Key.D8] = 0x38,
        [Key.D9] = 0x39,

        // 小键盘数字键 (0x60-0x69)
        [Key.NumPad0] = 0x60, [Key.NumPad1] = 0x61,
        [Key.NumPad2] = 0x62, [Key.NumPad3] = 0x63,
        [Key.NumPad4] = 0x64, [Key.NumPad5] = 0x65,
        [Key.NumPad6] = 0x66, [Key.NumPad7] = 0x67,
        [Key.NumPad8] = 0x68, [Key.NumPad9] = 0x69,
        [Key.Multiply] = 0x6A, [Key.Add] = 0x6B,
        [Key.Sleep] = 0x5F, [Key.Separator] = 0x6C,
        [Key.Subtract] = 0x6D, [Key.Decimal] = 0x6E,
        [Key.Divide] = 0x6F,
        

        // 功能键 F1-F24 (0x70-0x87)
        [Key.F1] = 0x70, [Key.F2] = 0x71, [Key.F3] = 0x72,
        [Key.F4] = 0x73, [Key.F5] = 0x74, [Key.F6] = 0x75,
        [Key.F7] = 0x76, [Key.F8] = 0x77, [Key.F9] = 0x78,
        [Key.F10] = 0x79, [Key.F11] = 0x7A, [Key.F12] = 0x7B,
        [Key.F13] = 0x7C, [Key.F14] = 0x7D, [Key.F15] = 0x7E,
        [Key.F16] = 0x7 ,[Key.F17] = 0x80, [Key.F18] = 0x81,
        [Key.F19] = 0x82, [Key.F20] = 0x83, [Key.F21] = 0x84,
        [Key.F22] = 0x85, [Key.F23] = 0x86, [Key.F24] = 0x87,

        [Key.NumLock] = 0x90,
        
        // 控制键
        [Key.LeftShift] = 0xA0, [Key.RightShift] = 0xA1,
        [Key.LeftCtrl] = 0xA2, [Key.RightCtrl] = 0xA3,
        [Key.LeftAlt] = 0xA4, [Key.RightAlt] = 0xA5,
        [Key.BrowserBack] = 0xA6, [Key.BrowserForward] = 0xA7,
        [Key.BrowserRefresh] = 0xA8,[Key.BrowserStop] = 0xA9,
        [Key.BrowserSearch] = 0xAA, [Key.BrowserHome] = 0xAC,
        [Key.MediaStop] = 0xB2, [Key.MediaPlayPause] = 0xB3,
        [Key.LaunchApplication1] = 0xB6, [Key.LaunchApplication2] = 0xB7,
        [Key.Oem1] = 0xBA, [Key.OemPlus] = 0xBB,
        [Key.OemComma] = 0xBC, [Key.OemMinus] = 0xBD,
        [Key.OemPeriod] = 0xBE, [Key.Oem2] = 0xBF,
        [Key.Oem3] = 0xC0, [Key.Oem4] = 0xDB,
        [Key.Oem5] = 0xDC, [Key.Oem6] = 0xDD,
        [Key.Oem7] = 0xDE, [Key.Oem8] = 0xDF,
        [Key.Oem102] = 0xE2, [Key.ImeProcessed] = 0xE5,
        [Key.Attn] = 0xF6, [Key.CrSel] = 0xF7,
        [Key.ExSel] = 0xF8, [Key.EraseEof] = 0xF9,
        [Key.Zoom] = 0xFB, [Key.NoName] = 0xFC, 
        [Key.Pa1] = 0xFD, [Key.OemClear] = 0xFE, 
        [Key.LWin] = 0x5B, [Key.RWin] = 0x5C,

        // 特殊键
        [Key.Enter] = 0x0D, [Key.Escape] = 0x1B,
        [Key.Space] = 0x20, [Key.Tab] = 0x09,
        [Key.Back] = 0x08, [Key.Delete] = 0x2E,
        [Key.PrintScreen] = 0x2C, [Key.Scroll] = 0x91,
        [Key.Pause] = 0x13, [Key.Insert] = 0x2D,
        [Key.Home] = 0x24, [Key.End] = 0x23,
        [Key.PageUp] = 0x21, [Key.PageDown] = 0x22,
        [Key.Left] =0x25, [Key.Up] = 0x26,
        [Key.Right] = 0x27, [Key.Down] = 0x28,

        // 多媒体键（需配合扩展标志）
        [Key.MediaNextTrack] = 0xB0,
        [Key.MediaPreviousTrack] = 0xB1,
        [Key.VolumeMute] = 0xAD,
        [Key.VolumeUp] = 0xAF,
        [Key.VolumeDown] = 0xAE,
        
        [Key.Cancel] = 0x03,
        [Key.Clear] = 0x0C,
        [Key.CapsLock] = 0x14,
        [Key.KanaMode] = 0x15,
        [Key.JunjaMode] = 0x17,
        [Key.FinalMode] = 0x18,
        [Key.HanjaMode] = 0x19,
        [Key.ImeConvert] = 0x1C,
        [Key.ImeNonConvert] = 0x1D,
        [Key.ImeAccept] = 0x1E,
        [Key.ImeModeChange] = 0x1F,
        [Key.Select] = 0x29,
        [Key.Print] = 0x2A,
        [Key.Execute] = 0x2B,
        [Key.Help] = 0x2F
    };

    // 扩展方法：动态转换未映射的键
    public static byte ToWin32VK(this Key key)
    {
        if (AvaloniaToWin32VK.TryGetValue(key, out byte vk))
            return vk;

        // 处理未映射的特殊键（如符号键需根据键盘布局转换）
        switch (key)
        {
            case Key.OemTilde:
                return 0xC0;    // ~ 键
            case Key.OemPipe:
                return 0xE2;    // \ 键（美式键盘）
            case Key.OemSemicolon:
                return 0xBA;    // ; 键
            case Key.OemQuotes:
                return 0xDE;    // ' 键
            default:
                Console.WriteLine($"未找到 {key} 的 Win32 映射");
                return 0;
        }
    }
}