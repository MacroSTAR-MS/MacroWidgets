using System;
using System.Runtime.InteropServices;

namespace MacWidget.Native;

/// <summary>
/// Win32 P/Invoke 声明，用于实现桌面置底窗口。
/// 参考 ClassIsland 的实现方式：通过 SetWindowPos(HWND_BOTTOM) 将窗口置底，
/// 并通过 WndProc 钩子监听 Z 序变化自动重新置底。
/// </summary>
public static class Win32Desktop
{
    // ──────────────────── P/Invoke 声明 ────────────────────

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    /// <summary>
    /// 链回原始 WndProc（关键：不能用 DefWindowProcW，否则 Avalonia 的渲染和输入全部丢失）
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr DefWindowProcW(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool ReleaseCapture();

    // ──────────────────── 常量 ────────────────────

    public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    public static readonly IntPtr HWND_TOP = new IntPtr(0);

    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_NOSENDCHANGING = 0x0100;
    public const uint SWP_NOOWNERZORDER = 0x0200;
    public const uint SWP_NOREPOSITION = 0x0200;

    public const int GWL_EXSTYLE = -20;
    public const int GWLP_WNDPROC = -4;

    public const int WS_EX_LAYERED = 0x00080000;
    public const int WS_EX_TRANSPARENT = 0x00000020;
    public const int WS_EX_NOACTIVATE = 0x08000000;
    public const int WS_EX_TOOLWINDOW = 0x00000040;
    public const int WS_EX_APPWINDOW = 0x00040000;
    public const int WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

    // 窗口消息
    public const uint WM_WINDOWPOSCHANGED = 0x0047;
    public const uint WM_NCHITTEST = 0x0084;
    public const uint WM_LBUTTONDOWN = 0x0201;
    public const uint WM_LBUTTONUP = 0x0202;
    public const uint WM_LBUTTONDBLCLK = 0x0203;
    public const uint WM_RBUTTONDOWN = 0x0204;
    public const uint WM_RBUTTONUP = 0x0205;
    public const uint WM_RBUTTONDBLCLK = 0x0206;
    public const uint WM_MBUTTONDOWN = 0x0207;
    public const uint WM_MBUTTONUP = 0x0208;
    public const uint WM_NCLBUTTONDOWN = 0x00A1;

    // 命中测试返回值
    public static readonly IntPtr HTTRANSPARENT = new IntPtr(-1);
    public static readonly IntPtr HTCLIENT = new IntPtr(1);

    public const uint GW_HWNDNEXT = 2;

    // ──────────────────── 结构体 ────────────────────

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public uint flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // ──────────────────── 公共方法 ────────────────────

    /// <summary>
    /// 将窗口置底（HWND_BOTTOM）。
    /// </summary>
    public static void SetWindowBottom(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return;

        SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0,
            SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE |
            SWP_NOSENDCHANGING | SWP_NOOWNERZORDER | SWP_NOREPOSITION);
    }

    /// <summary>
    /// 设置窗口为工具窗口（不在 Alt+Tab 和任务栏中显示）。
    /// </summary>
    public static void SetToolWindow(IntPtr hWnd, bool enable)
    {
        if (hWnd == IntPtr.Zero) return;

        var style = GetWindowLongPtr(hWnd, GWL_EXSTYLE).ToInt64();
        if (enable)
            style |= WS_EX_TOOLWINDOW;
        else
            style &= ~WS_EX_TOOLWINDOW;

        SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(style));
    }

    /// <summary>
    /// 设置窗口为不激活（点击时不抢焦点）。
    /// </summary>
    public static void SetNoActivate(IntPtr hWnd, bool enable)
    {
        if (hWnd == IntPtr.Zero) return;

        var style = GetWindowLongPtr(hWnd, GWL_EXSTYLE).ToInt64();
        if (enable)
            style |= WS_EX_NOACTIVATE;
        else
            style &= ~WS_EX_NOACTIVATE;

        SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(style));
    }

    /// <summary>
    /// 检查 WINDOWPOS 中的 Z 序是否变化（用于 WndProc 钩子）。
    /// </summary>
    public static bool IsZOrderChanging(ref WINDOWPOS pos)
    {
        return (pos.flags & SWP_NOZORDER) == 0;
    }

    /// <summary>
    /// 查找桌面 ListView 窗口。
    /// Windows 桌面图标由 "SHELLDLL_DefView" 类的窗口管理，
    /// 它是桌面 "Progman" 或 "WorkerW" 窗口的子窗口。
    /// 鼠标消息转发到这里可以让桌面图标正常接收点击。
    /// </summary>
    public static IntPtr FindDesktopListView()
    {
        // 方式1：直接在 Progman 下查找
        var progman = FindWindow("Progman", null!);
        if (progman != IntPtr.Zero)
        {
            var shellView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellView != IntPtr.Zero)
                return shellView;
        }

        // 方式2：遍历 WorkerW 查找（有些系统桌面在 WorkerW 下）
        var workerW = IntPtr.Zero;
        while (true)
        {
            workerW = FindWindowEx(IntPtr.Zero, workerW, "WorkerW", null);
            if (workerW == IntPtr.Zero) break;

            var shellView = FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellView != IntPtr.Zero)
                return shellView;
        }

        return IntPtr.Zero;
    }
}
