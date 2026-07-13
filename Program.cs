using Avalonia;
using System;
using System.Threading;

namespace MacroWidgets;

class Program
{
    private static Mutex? _mutex;

    [STAThread]
    public static void Main(string[] args)
    {
        // 单实例锁
        _mutex = new Mutex(true, "Global\\MacroWidgets_SingleInstance", out bool createdNew);
        if (!createdNew) return;

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
