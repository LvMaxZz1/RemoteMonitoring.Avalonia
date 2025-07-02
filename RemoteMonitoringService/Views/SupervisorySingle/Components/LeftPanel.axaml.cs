using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(LeftPanelViewModel))]
public partial class LeftPanel : BaseUserControl<LeftPanelViewModel>
{
    // 用于记录每个TextBlock的动画token
    private readonly Dictionary<TextBlock, CancellationTokenSource> _shakeTokens = new();
    
    public LeftPanel()
    {
        InitializeComponent();
       
    }

    protected override void OnLoaded(RoutedEventArgs e)
    { 
        var listBox = this.FindControl<ListBox>("ManuListBox");
        if (listBox != null)
        {
            // 先解绑，防止重复
            foreach (var item in listBox.GetVisualDescendants().OfType<ListBoxItem>())
            {
                item.PointerEntered -= ListBoxItem_PointerEnter;
                item.PointerExited -= ListBoxItem_PointerExited;
                item.PointerEntered += ListBoxItem_PointerEnter;
                item.PointerExited += ListBoxItem_PointerExited;
            }
        }
        base.OnLoaded(e);
    }
    
    private void ListBoxItem_PointerEnter(object? sender, PointerEventArgs e)
    {
        if (sender is ListBoxItem item)
        {
            var tb = item.GetVisualDescendants()
                .OfType<TextBlock>()
                .FirstOrDefault(x => x.Name == "ListBoxTextBlock");
            if (tb != null)
            {
                AnimatedText_PointerEnter(tb, e);
            }
        }
    }

    private void ListBoxItem_PointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is ListBoxItem item)
        {
            var tb = item.GetVisualDescendants()
                .OfType<TextBlock>()
                .FirstOrDefault(x => x.Name == "ListBoxTextBlock");
            if (tb != null)
            {
                AnimatedText_PointerExited(tb, e);
            }
        }
    }
    
    private void AnimatedText_PointerEnter(object? sender, PointerEventArgs e)
    {
        if (sender is TextBlock tb)
        {
            // 停止之前的动画
            if (_shakeTokens.TryGetValue(tb, out var oldCts))
            {
                oldCts.Cancel();
                _shakeTokens.Remove(tb);
            }

            var cts = new CancellationTokenSource();
            _shakeTokens[tb] = cts;
            StartShake(tb, cts.Token);
        }
    }

    private void AnimatedText_PointerExited(object? sender, PointerEventArgs pointerEventArgs)
    {
        if (sender is TextBlock tb)
        {
            if (_shakeTokens.TryGetValue(tb, out var cts))
            {
                cts.Cancel();
                _shakeTokens.Remove(tb);
            }
            // 恢复到原位
            if (tb.RenderTransform is TranslateTransform tt)
            {
                tt.X = 0;
            }
        }
    }

    private async void StartShake(TextBlock tb, CancellationToken token)
    {
        if (tb.RenderTransform is not TranslateTransform tt)
            return;

        const int from = -7;
        const int to = 10;
        const int duration = 400; // 单边晃动时间ms
        const int totalFrames = duration / 16; // 约60fps
        var forward = true;

        try
        {
            while (!token.IsCancellationRequested)
            {
                for (var frame = 0; frame < totalFrames && !token.IsCancellationRequested; frame++)
                {
                    var t = (double)frame / totalFrames;
                    // 线性插值，匀速
                    var value = forward
                        ? from + (to - from) * t
                        : to - (to - from) * t;
                    await Dispatcher.UIThread.InvokeAsync(() => tt.X = value);
                    await Task.Delay(16, token); // 约60fps
                }
                forward = !forward;
            }
        }
        catch (TaskCanceledException)
        {
            // 被取消时什么都不用做
        }
        finally
        {
            // 平滑回中间
            var start = tt.X;
            const int end = 0;
            const int backFrames = 10;
            for (var i = 0; i < backFrames; i++)
            {
                if (token.IsCancellationRequested) break;
                var t = (double)i / backFrames;
                // 线性插值
                var value = start + (end - start) * t;
                await Dispatcher.UIThread.InvokeAsync(() => tt.X = value);
                await Task.Delay(16, token);
            }
            await Dispatcher.UIThread.InvokeAsync(() => tt.X = 0);
        }
    }
}