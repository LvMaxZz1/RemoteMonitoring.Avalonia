using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringConsole.Views;

public partial class FullScreenWindow : BaseWindow<MonitoringBoardPanelViewModel>
{
    private bool _lastLeftPressed;
    private bool _lastRightPressed;
    private bool _lastMiddlePressed;

    public FullScreenWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        ViewModel.IsOpenFullScreen = false;
        base.OnClosed(e);
    }

    private async void InputElement_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (ViewModel.ConsoleNetworkService.ConsoleLinkChannel == null || ViewModel.Power == Power.Off || Monitor.Source == null)
            return;
        // point 等于 pointerPoint
        //dpiScaling 是屏幕缩放比例

        // 获取与控件相关的事件内指针的坐标
        var pointerPoint = e.GetCurrentPoint(Monitor);
        // 控件内指针坐标映射到屏幕坐标系
        var screenPoint = VisualExtensions.PointToScreen(Monitor, pointerPoint.Position);
        //  获取主屏幕
        var primaryScreen = this.Screens.All.First(s => s.IsPrimary);
        // 本机缩放比例
        var dpiScaling = primaryScreen.Scaling;
        // 主屏幕的XY， 当前为0  无效果 应该用Width和Height
        var primaryScreenOffsetX = primaryScreen.Bounds.X; // 替换原来的 Left
        var primaryScreenOffsetY = primaryScreen.Bounds.Y; // 替换原来的 Top

        // 计算出缩放比例之后的主屏幕宽和高
        var physicalWidth = primaryScreen.Bounds.Width * dpiScaling; // 物理像素宽度
        var physicalHeight = primaryScreen.Bounds.Height * dpiScaling; // 物理像素高度

        // 计算出缩放比例之后的控件内屏幕坐标
        var physicalX = screenPoint.X * dpiScaling;
        var physicalY = screenPoint.Y * dpiScaling;


        var dx2 = (int)(physicalX * 65535 / Monitor.Bounds.Width);
        var dy2 = (int)(physicalY * 65535 / Monitor.Bounds.Height);

        // 鼠标在自己的屏幕那里就会到目标屏幕那里
        var dx3 = (int)(physicalX / physicalWidth * 65535);
        var dy3 = (int)(physicalY / physicalHeight * 65535);
        //
        // Console.WriteLine(
        //     $"{screenPoint.X},{screenPoint.Y},{primaryScreenOffsetX},{primaryScreenOffsetY},{dpiScaling}");
        // Console.WriteLine($"{physicalX},{physicalY}");
        // Console.WriteLine($"{dx2},{dy2}");

        // 获取控件坐标系的指针
        var point = e.GetPosition(Monitor);
        // 鼠标在监控那里，就会点到那里
        var dx = (int)(point.X * 65535 / Monitor.Bounds.Width);
        var dy = (int)(point.Y * 65535 / Monitor.Bounds.Height);
        var dwFlags = (int)(MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE);
        var screen = new ScreenInfo(ViewModel.Power, 50)
        {
            Mouse = new Mouse
            {
                DwFlagsOne = dwFlags,
                Dx = dx2,
                Dy = dy2
            }
        };
        await ViewModel.SendCommandToClient(CommandType.ObtainScreen, screen);
    }

    private async void Monitor_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (ViewModel.ConsoleNetworkService.ConsoleLinkChannel == null || ViewModel.Power == Power.Off || Monitor.Source == null)
            return;
        // point 等于 pointerPoint
        //dpiScaling 是屏幕缩放比例
        // 获取与当前事件关联的指针(参数为以什么控件为坐标系)
        var pointerPoint = e.GetCurrentPoint(Monitor);
        // 将鼠标坐标以控件坐标系转化为以屏幕为坐标系（缩放后的）
        var screenPoint = VisualExtensions.PointToScreen(Monitor, pointerPoint.Position);
        // 获取主屏幕信息
        var primaryScreen = this.Screens.All.First(s => s.IsPrimary);
        // 获取缩放比例
        var dpiScaling = primaryScreen.Scaling;
        // 计算出缩放比例之后的主屏幕宽和高
        var physicalWidth = primaryScreen.Bounds.Width * dpiScaling; // 物理像素宽度
        var physicalHeight = primaryScreen.Bounds.Height * dpiScaling; // 物理像素高度
        var physicalX = screenPoint.X * dpiScaling;
        var physicalY = screenPoint.Y * dpiScaling;


        var dx2 = (int)(physicalX * 65535 / Monitor.Bounds.Width);
        var dy2 = (int)(physicalY * 65535 / Monitor.Bounds.Height);

        // 鼠标在自己的屏幕那里就会到目标屏幕那里
        var dx3 = (int)(physicalX / physicalWidth * 65535);
        var dy3 = (int)(physicalY / physicalHeight * 65535);

        Console.WriteLine($"{screenPoint.X},{screenPoint.Y},,{dpiScaling}");
        Console.WriteLine($"{physicalX},{physicalY}");
        Console.WriteLine($"{dx2},{dy2}");
        var point = e.GetPosition(Monitor);
        // 鼠标在监控那里，就会点到那里
        var dx = (int)(point.X * 65535 / Monitor.Bounds.Width);
        var dy = (int)(point.Y * 65535 / Monitor.Bounds.Height);
        var dwFlags = (int)(MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE);
        var screen = new ScreenInfo(ViewModel.Power, 50)
        {
            Mouse = new Mouse
            {
                DwFlagsOne = dwFlags,
                Dx = dx,
                Dy = dy
            }
        };
        await ViewModel.SendCommandToClient(CommandType.ObtainScreen, screen);
        await Task.Delay(40);
    }

    private async void Monitor_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (ViewModel.ConsoleNetworkService.ConsoleLinkChannel == null || ViewModel.Power == Power.Off || Monitor.Source == null)
            return;
        // point 等于 pointerPoint
        //dpiScaling 是屏幕缩放比例
        // 获取与当前事件关联的指针(参数为以什么控件为坐标系)
        var pointerPoint = e.GetCurrentPoint(Monitor);
        // 将鼠标坐标以控件坐标系转化为以屏幕为坐标系（缩放后的）
        var screenPoint = VisualExtensions.PointToScreen(Monitor, pointerPoint.Position);
        // 获取主屏幕信息
        var primaryScreen = this.Screens.All.First(s => s.IsPrimary);
        // 获取缩放比例
        var dpiScaling = primaryScreen.Scaling;

        // 计算出缩放比例之后的主屏幕宽和高
        var physicalWidth = primaryScreen.Bounds.Width * dpiScaling; // 物理像素宽度
        var physicalHeight = primaryScreen.Bounds.Height * dpiScaling; // 物理像素高度
        // 此处应该不需要再进行缩放，因为通过空间获取屏幕坐标应该已经缩放过 待测试
        var physicalX = screenPoint.X * dpiScaling;
        var physicalY = screenPoint.Y * dpiScaling;


        var dx2 = (int)(physicalX * 65535 / Monitor.Bounds.Width);
        var dy2 = (int)(physicalY * 65535 / Monitor.Bounds.Height);

        // 鼠标在自己的屏幕那里就会到目标屏幕那里
        var dx3 = (int)(physicalX / physicalWidth * 65535);
        var dy3 = (int)(physicalY / physicalHeight * 65535);

        var point = e.GetPosition(Monitor);
        // 鼠标在监控那里，就会点到那里
        var dx = (int)(point.X * 65535 / Monitor.Bounds.Width);
        var dy = (int)(point.Y * 65535 / Monitor.Bounds.Height);
        MOUSEEVENTF dwFlags = 0;
        // 独立判断各键按下状态（非互斥）
        if (pointerPoint.Properties.IsLeftButtonPressed)
        {
            dwFlags |= MOUSEEVENTF.LEFTDOWN;
        }

        if (pointerPoint.Properties.IsRightButtonPressed)
        {
            dwFlags |= MOUSEEVENTF.RIGHTDOWN;
        }

        if (pointerPoint.Properties.IsMiddleButtonPressed)
        {
            dwFlags |= MOUSEEVENTF.MIDDLEDOWN;
        }

        // 独立判断各键松开状态
        if (!pointerPoint.Properties.IsLeftButtonPressed && _lastLeftPressed)
        {
            dwFlags |= MOUSEEVENTF.LEFTUP;
        }

        if (!pointerPoint.Properties.IsRightButtonPressed && _lastRightPressed)
        {
            dwFlags |= MOUSEEVENTF.RIGHTUP;
        }

        if (!pointerPoint.Properties.IsMiddleButtonPressed && _lastMiddlePressed)
        {
            dwFlags |= MOUSEEVENTF.MIDDLEUP;
        }

        // 更新最后状态记录
        _lastLeftPressed = pointerPoint.Properties.IsLeftButtonPressed;
        _lastRightPressed = pointerPoint.Properties.IsRightButtonPressed;
        _lastMiddlePressed = pointerPoint.Properties.IsMiddleButtonPressed;

        var screen = new ScreenInfo(ViewModel.Power, 50)
        {
            Mouse = new Mouse
            {
                DwFlagsOne = (int)dwFlags,
                Dx = dx2,
                Dy = dy2
            }
        };
        await ViewModel.SendCommandToClient(CommandType.ObtainScreen, screen);
    }

    private async void Monitor_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (ViewModel.ConsoleNetworkService.ConsoleLinkChannel == null || ViewModel.Power == Power.Off || Monitor.Source == null)
            return;
        // point 等于 pointerPoint
        //dpiScaling 是屏幕缩放比例
        // 获取与当前事件关联的指针(参数为以什么控件为坐标系)
        var pointerPoint = e.GetCurrentPoint(Monitor);
        // 将鼠标坐标以控件坐标系转化为以屏幕为坐标系（缩放后的）
        var screenPoint = VisualExtensions.PointToScreen(Monitor, pointerPoint.Position);
        // 获取主屏幕信息
        var primaryScreen = this.Screens.All.First(s => s.IsPrimary);
        // 获取缩放比例
        var dpiScaling = primaryScreen.Scaling;

        // 计算出缩放比例之后的主屏幕宽和高
        var physicalWidth = primaryScreen.Bounds.Width * dpiScaling; // 物理像素宽度
        var physicalHeight = primaryScreen.Bounds.Height * dpiScaling; // 物理像素高度
        var physicalX = screenPoint.X * dpiScaling;
        var physicalY = screenPoint.Y * dpiScaling;


        var dx2 = (int)(physicalX * 65535 / Monitor.Bounds.Width);
        var dy2 = (int)(physicalY * 65535 / Monitor.Bounds.Height);

        // 鼠标在自己的屏幕那里就会到目标屏幕那里
        var dx3 = (int)(physicalX / physicalWidth * 65535);
        var dy3 = (int)(physicalY / physicalHeight * 65535);

        var point = e.GetPosition(Monitor);
        // 鼠标在监控那里，就会点到那里
        var dx = (int)(point.X * 65535 / Monitor.Bounds.Width);
        var dy = (int)(point.Y * 65535 / Monitor.Bounds.Height);
        MOUSEEVENTF dwFlags = 0;
        // 独立判断各键按下状态（非互斥）
        if (pointerPoint.Properties.IsLeftButtonPressed)
        {
            dwFlags |= MOUSEEVENTF.LEFTDOWN;
        }

        if (pointerPoint.Properties.IsRightButtonPressed)
        {
            dwFlags |= MOUSEEVENTF.RIGHTDOWN;
        }

        if (pointerPoint.Properties.IsMiddleButtonPressed)
        {
            dwFlags |= MOUSEEVENTF.MIDDLEDOWN;
        }

        // 独立判断各键松开状态
        if (!pointerPoint.Properties.IsLeftButtonPressed && _lastLeftPressed)
        {
            dwFlags |= MOUSEEVENTF.LEFTUP;
        }

        if (!pointerPoint.Properties.IsRightButtonPressed && _lastRightPressed)
        {
            dwFlags |= MOUSEEVENTF.RIGHTUP;
        }

        if (!pointerPoint.Properties.IsMiddleButtonPressed && _lastMiddlePressed)
        {
            dwFlags |= MOUSEEVENTF.MIDDLEUP;
        }

        // 更新最后状态记录
        _lastLeftPressed = pointerPoint.Properties.IsLeftButtonPressed;
        _lastRightPressed = pointerPoint.Properties.IsRightButtonPressed;
        _lastMiddlePressed = pointerPoint.Properties.IsMiddleButtonPressed;

        var screen = new ScreenInfo(ViewModel.Power, 50)
        {
            Mouse = new Mouse
            {
                DwFlagsOne = (int)dwFlags,
                Dx = dx2,
                Dy = dy2
            }
        };
        await ViewModel.SendCommandToClient(CommandType.ObtainScreen, screen);
    }

    private async void Monitor_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (ViewModel.ConsoleNetworkService.ConsoleLinkChannel == null || ViewModel.Power == Power.Off || Monitor.Source == null)
            return;
        // point 等于 pointerPoint
        //dpiScaling 是屏幕缩放比例
        // 获取与当前事件关联的指针(参数为以什么控件为坐标系)
        var pointerPoint = e.GetPosition(Monitor);
        // 将鼠标坐标以控件坐标系转化为以屏幕为坐标系（缩放后的）
        var screenPoint = VisualExtensions.PointToScreen(Monitor, pointerPoint);
        // 获取主屏幕信息
        var primaryScreen = this.Screens.All.First(s => s.IsPrimary);
        // 获取缩放比例
        var dpiScaling = primaryScreen.Scaling;

        // 计算出缩放比例之后的主屏幕宽和高
        var physicalWidth = primaryScreen.Bounds.Width * dpiScaling; // 物理像素宽度
        var physicalHeight = primaryScreen.Bounds.Height * dpiScaling; // 物理像素高度
        // 此处应该不需要再进行缩放，因为通过空间获取屏幕坐标应该已经缩放过
        var physicalX = screenPoint.X * dpiScaling;
        var physicalY = screenPoint.Y * dpiScaling;


        var dx2 = (int)(physicalX * 65535 / Monitor.Bounds.Width);
        var dy2 = (int)(physicalY * 65535 / Monitor.Bounds.Height);

        // 鼠标在自己的屏幕那里就会到目标屏幕那里
        var dx3 = (int)(physicalX / physicalWidth * 65535);
        var dy3 = (int)(physicalY / physicalHeight * 65535);

        var point = e.GetPosition(Monitor);
        // 鼠标在监控那里，就会点到那里
        var dx = (int)(point.X * 65535 / Monitor.Bounds.Width);
        var dy = (int)(point.Y * 65535 / Monitor.Bounds.Height);
        var dwFlagsOne = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTDOWN;
        var dwFlagsTwo = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTUP;

        var screen = new ScreenInfo(ViewModel.Power, 50)
        {
            Mouse = new Mouse
            {
                DwFlagsOne = (int)dwFlagsOne,
                DwFlagsTwo = (int)dwFlagsTwo,
                Dx = dx2,
                Dy = dy2,
                IsDouble = true
            }
        };
        await ViewModel.SendCommandToClient(CommandType.ObtainScreen, screen);
    }

    private async void Monitor_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (ViewModel.ConsoleNetworkService.ConsoleLinkChannel == null || ViewModel.Power == Power.Off || Monitor.Source == null)
            return;
        var screen = new ScreenInfo(ViewModel.Power, 50)
        {
            KeyBd = new Keybd
            {
                BVk = e.Key.ToWin32VK(),
                BScan = 0,
                DwFlags = 0
            }
        };
        await ViewModel.SendCommandToClient(CommandType.ObtainScreen, screen);
    }

    private async void Monitor_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (ViewModel.ConsoleNetworkService.ConsoleLinkChannel == null || ViewModel.Power == Power.Off || Monitor.Source == null)
            return;
        var screen = new ScreenInfo(ViewModel.Power, 50)
        {
            KeyBd = new Keybd
            {
                BVk = e.Key.ToWin32VK(),
                BScan = 0,
                DwFlags = (int)KEYEVENTF.KEYUP
            }
        };
        await ViewModel.SendCommandToClient(CommandType.ObtainScreen, screen);
    }
}