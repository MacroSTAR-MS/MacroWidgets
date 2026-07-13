using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using MacWidget.Native;

namespace MacWidget.Views;

/// <summary>
/// 独立的桌面小组件窗口。
/// 每个小组件都是一个小窗口，只覆盖小组件本身的区域，
/// 桌面空白区域没有任何窗口遮挡，天然不需要点击穿透。
/// </summary>
public class WidgetWindow : Window
{
    private IntPtr _hwnd = IntPtr.Zero;

    /// <summary>
    /// 小组件类型标识（如 "clock", "weather"）
    /// </summary>
    public string WidgetType { get; }

    /// <summary>
    /// 窗口关闭时通知外部进行清理
    /// </summary>
    public event Action<WidgetWindow>? ClosedByUser;

    public WidgetWindow(UserControl widget, string title, string widgetType, int x, int y, int width, int height)
    {
        WidgetType = widgetType;
        Title = title;
        Width = width;
        Height = height;
        SystemDecorations = SystemDecorations.None;
        Background = Brushes.Transparent;
        TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
        CanResize = false;
        ShowInTaskbar = false;
        WindowStartupLocation = WindowStartupLocation.Manual;

        Content = widget;

        // 左键拖动
        PointerPressed += OnPointerPressed;

        // 右键菜单
        var contextMenu = new ContextMenu();
        var removeItem = new MenuItem { Header = "移除此小组件" };
        removeItem.Click += (_, _) => RemoveWidget();
        contextMenu.Items?.Add(removeItem);
        ContextMenu = contextMenu;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // 设置初始位置
        Position = new PixelPoint(Position.X, Position.Y);

        var handle = this.TryGetPlatformHandle();
        if (handle?.HandleDescriptor is string desc && desc.StartsWith("HWND"))
        {
            _hwnd = handle.Handle;

            var exStyle = Win32Desktop.GetWindowLongPtr(_hwnd, Win32Desktop.GWL_EXSTYLE).ToInt64();
            exStyle |= Win32Desktop.WS_EX_TOOLWINDOW;
            exStyle |= Win32Desktop.WS_EX_NOACTIVATE;
            Win32Desktop.SetWindowLongPtr(_hwnd, Win32Desktop.GWL_EXSTYLE, new IntPtr(exStyle));

            Win32Desktop.SetWindowBottom(_hwnd);
            InstallWndProcHook();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            Win32Desktop.ReleaseCapture();
            Win32Desktop.SendMessage(_hwnd,
                Win32Desktop.WM_NCLBUTTONDOWN,
                new IntPtr(2), // HTCAPTION
                IntPtr.Zero);
        }
    }

    private void RemoveWidget()
    {
        ClosedByUser?.Invoke(this);
        Close();
    }

    /// <summary>
    /// 获取当前窗口的桌面位置和尺寸（用于持久化）
    /// </summary>
    public WidgetLayoutInfo GetLayoutInfo()
    {
        return new WidgetLayoutInfo
        {
            Type = WidgetType,
            X = Position.X,
            Y = Position.Y,
            Width = (int)Width,
            Height = (int)Height
        };
    }

    // ── WndProc 钩子：保持置底 ──
    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private WndProcDelegate? _newWndProc;
    private IntPtr _originalWndProc = IntPtr.Zero;

    private void InstallWndProcHook()
    {
        if (_hwnd == IntPtr.Zero) return;
        _newWndProc = new WndProcDelegate(HookWndProc);
        _originalWndProc = Win32Desktop.GetWindowLongPtr(_hwnd, Win32Desktop.GWLP_WNDPROC);
        var funcPtr = Marshal.GetFunctionPointerForDelegate(_newWndProc);
        Win32Desktop.SetWindowLongPtr(_hwnd, Win32Desktop.GWLP_WNDPROC, funcPtr);
    }

    private IntPtr HookWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == Win32Desktop.WM_WINDOWPOSCHANGED)
        {
            var pos = Marshal.PtrToStructure<Win32Desktop.WINDOWPOS>(lParam);
            if (Win32Desktop.IsZOrderChanging(ref pos) && pos.hwndInsertAfter != Win32Desktop.HWND_BOTTOM)
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    Win32Desktop.SetWindowBottom(_hwnd));
            }
        }

        if (_originalWndProc != IntPtr.Zero)
            return Win32Desktop.CallWindowProc(_originalWndProc, hWnd, msg, wParam, lParam);

        return Win32Desktop.DefWindowProcW(hWnd, msg, wParam, lParam);
    }
}

/// <summary>
/// 小组件布局信息，用于 JSON 持久化
/// </summary>
public class WidgetLayoutInfo
{
    public string Type { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
