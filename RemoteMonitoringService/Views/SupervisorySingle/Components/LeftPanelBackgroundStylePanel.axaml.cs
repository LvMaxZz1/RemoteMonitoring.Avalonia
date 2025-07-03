using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

public partial class LeftPanelBackgroundStylePanel : UserControl
{
    private double _time;
    private readonly double _amplitude = 5; // 波浪幅度
    private readonly double _frequency = 2.0; // 波浪频率
    private readonly double _speed = 2.0; // 波浪速度
    private readonly int _pointCount = 80; // 波浪点数
    private IDisposable? _timer;

    public LeftPanelBackgroundStylePanel()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (_, __) => StartWaveAnimation();
        this.DetachedFromVisualTree += (_, __) => _timer?.Dispose();
    }

    private void StartWaveAnimation()
    {
        _timer = DispatcherTimer.Run(UpdateWave, TimeSpan.FromMilliseconds(10));
    }

    private bool UpdateWave()
    {
        _time += _speed * 0.01; // 100FPS

        var width = RootCanvas.Bounds.Width;
        var height = RootCanvas.Bounds.Height;
        if (width == 0 || height == 0)
            return true;

        // 1. 生成顶部波浪点
        var sb = new StringBuilder();
        sb.Append($"M 0,{_amplitude} ");
        for (int i = 0; i < _pointCount; i++)
        {
            var x = i * width / (_pointCount - 1);
            var y = _amplitude + _amplitude * Math.Sin(_frequency * (x / width * 2 * Math.PI) + _time);
            sb.Append($"L {x},{y} ");
        }
        // 2. 右上角、右下角、左下角、回到起点
        sb.Append($"L {width},{height} ");
        sb.Append($"L 0,{height} ");
        sb.Append("Z");

        // 3. 设置Path
        WavePath.Data = Geometry.Parse(sb.ToString());
        return true;
    }
}