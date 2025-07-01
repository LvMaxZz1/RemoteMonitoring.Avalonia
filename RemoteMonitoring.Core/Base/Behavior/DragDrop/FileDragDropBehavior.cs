using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace RemoteMonitoring.Core.Base.Behavior.DragDrop;

[Description("文件拖放")]
public class FileDragDropBehavior
{
    [Description("是否启用")]
    private static readonly AttachedProperty<bool> IsEnabledProperty = 
        AvaloniaProperty.RegisterAttached<FileDragDropBehavior, Control, bool>(
            "IsEnabled", 
            defaultValue: false);
    
    public static bool GetIsEnabled(AvaloniaObject obj) 
        => obj.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(AvaloniaObject obj, bool value) 
        => obj.SetValue(IsEnabledProperty, value);
    
    private static readonly ConditionalWeakTable<Control, Subscription> Subscriptions = new();

    static FileDragDropBehavior()
    {
        IsEnabledProperty.Changed.Subscribe(args =>
        {
            if (args.Sender is Control control)
            {
                if (args.NewValue.Value)
                {
                    var sub = new Subscription(control);
                    Subscriptions.Add(control, sub);
                }
                else if (Subscriptions.TryGetValue(control, out var sub))
                {
                    sub.Dispose();
                    Subscriptions.Remove(control);
                }
            }
        });
    }

    [Description("拖拽文件过程中")]
    private static void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    [Description("拖拽文件结束")]
    private static async void OnDrop(object? sender, DragEventArgs e)
    {
        if (sender is Control { DataContext: IFileTransfer vm } control &&
            Subscriptions.TryGetValue(control, out var sub))
        {
            var files = e.Data.GetFiles()?.ToList();
            if (files != null && files.Count != 0)
            {
                await Parallel.ForEachAsync(files, sub.Cts.Token, async (file, ct) =>
                {
                    await vm.FileTransferAsync(file, ct);
                });
            }
            e.Handled = true;
        }
    }
    
    [Description("控件离开可视化树")]
    private static void OnControlDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Control control)
        {
            control.RemoveHandler(Avalonia.Input.DragDrop.DragOverEvent, OnDragOver);
            control.RemoveHandler(Avalonia.Input.DragDrop.DropEvent, OnDrop);
            control.DetachedFromVisualTree -= OnControlDetached;
        }
    }
    
    private sealed class Subscription : IDisposable
    {
        private readonly Control _control;
        public readonly CancellationTokenSource Cts = new();

        public Subscription(Control control)
        {
            _control = control;
            
            Avalonia.Input.DragDrop.SetAllowDrop(control, true);
            control.AddHandler(Avalonia.Input.DragDrop.DragOverEvent, OnDragOver);
            control.AddHandler(Avalonia.Input.DragDrop.DropEvent, OnDrop);
            control.DetachedFromVisualTree += OnControlDetached;
        }

        public void Dispose()
        {
            Cts.Cancel();
            _control.RemoveHandler(Avalonia.Input.DragDrop.DragOverEvent, OnDragOver);
            _control.RemoveHandler(Avalonia.Input.DragDrop.DropEvent, OnDrop);
            _control.DetachedFromVisualTree -= OnControlDetached;
            Avalonia.Input.DragDrop.SetAllowDrop(_control, false);
        }
    }
}