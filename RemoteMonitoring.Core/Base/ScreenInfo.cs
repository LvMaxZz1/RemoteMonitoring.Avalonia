using System.ComponentModel;

namespace RemoteMonitoring.Core.Base;

[Description("屏幕信息")]
public class ScreenInfo(Power power, int quality, Keybd? keyBd = null, Mouse? mouse = null)
{
    private readonly int _quality = quality;

    [Description("图像质量(0~100)")]
    public int Quality { get; set; }
    
    public Power? Power { get; set; } = power;

    public  Keybd? KeyBd { get; set; } = keyBd;

    public  Mouse Mouse { get; set; } = mouse;
}

[Description("键盘")]
public class Keybd
{
    public byte BVk { get; set; }
    
    public byte BScan { get; set; }
    
    public int DwFlags { get; set; }
}

[Description("鼠标")]
public class Mouse
{
    public int DwFlagsOne { get; set; }
    
    public int DwFlagsTwo { get; set; }
    
    public int Dx { get; set; }
    
    public int Dy { get; set; }
    
    public int DwData { get; set; }

    public bool IsDouble { get; set; }
}

public enum Power : byte
{
    Off = 0x0,
    
    On = 0x1
}