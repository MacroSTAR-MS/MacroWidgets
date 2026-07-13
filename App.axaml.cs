using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using MacroWidgets.Services;
using MacroWidgets.Views;
using MacroWidgets.Views.Gallery;
using MacroWidgets.Views.Settings;
using MacroWidgets.Views.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MacroWidgets;

public partial class App : Application
{
    public App()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }

    private TrayIcon? _trayIcon;
    private WidgetGalleryWindow? _galleryWindow;
    private SettingsWindow? _settingsWindow;
    private readonly List<WidgetWindow> _widgetWindows = new();
    private static int _nextWidgetOffset = 500;

    /// <summary>
    /// 持久化服务（单例）
    /// </summary>
    public static WidgetPersistenceService Persistence { get; } = new();

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // 加载持久化配置
            Persistence.Load();

            // 创建小组件窗口
            if (Persistence.Config.Widgets.Count > 0)
            {
                // 从配置恢复
                foreach (var w in Persistence.Config.Widgets)
                {
                    CreateWidgetWindow(w.Type, w.X, w.Y, w.Width, w.Height);
                }
            }
            else
            {
                // 首次运行，创建默认小组件
                CreateWidgetWindow("clock", 60, 60, 280, 170);
                CreateWidgetWindow("weather", 60, 250, 300, 260);
                CreateWidgetWindow("calendar", 420, 60, 280, 340);
                CreateWidgetWindow("reminders", 420, 420, 280, 210);
            }

            desktop.MainWindow = _widgetWindows.FirstOrDefault() ??
                CreateWidgetWindow("clock", 60, 60, 280, 170);

            InitializeTrayIcon();

            // 退出时保存布局
            desktop.Exit += (_, _) => SaveLayout();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// 创建一个小组件窗口并跟踪
    /// </summary>
    private WidgetWindow CreateWidgetWindow(string type, int x, int y, int w, int h)
    {
        var widget = WidgetManager.Instance.CreateWidget(type);
        var name = WidgetManager.Instance.AvailableWidgets
            .Find(e => e.Type == type)?.Name ?? type;

        var window = new WidgetWindow(widget, name, type, x, y, w, h);
        window.ClosedByUser += OnWidgetClosed;
        window.Closed += (_, _) => _widgetWindows.Remove(window);
        _widgetWindows.Add(window);
        return window;
    }

    private void OnWidgetClosed(WidgetWindow w)
    {
        _widgetWindows.Remove(w);
        SaveLayout();
    }

    /// <summary>
    /// 保存当前所有小组件的布局
    /// </summary>
    private void SaveLayout()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var layout = _widgetWindows.Select(w => w.GetLayoutInfo()).ToList();
            Persistence.SaveLayout(layout);
        });
    }

    private void InitializeTrayIcon()
    {
        var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "tray_icon.ico");
        var icon = new WindowIcon(iconPath);

        var menu = new NativeMenu();

        var openGalleryItem = new NativeMenuItem("打开小组件画廊");
        openGalleryItem.Click += (_, _) => OpenGallery();
        menu.Items.Add(openGalleryItem);

        var settingsItem = new NativeMenuItem("设置");
        settingsItem.Click += (_, _) => OpenSettings();
        menu.Items.Add(settingsItem);

        menu.Items.Add(new NativeMenuItemSeparator());

        var exitItem = new NativeMenuItem("退出");
        exitItem.Click += (_, _) => ExitApp();
        menu.Items.Add(exitItem);

        _trayIcon = new TrayIcon
        {
            Icon = icon,
            ToolTipText = "MacroWidgets 桌面小组件",
            Menu = menu
        };
        _trayIcon.Clicked += (_, _) => OpenGallery();

        this.SetValue(TrayIcon.IconsProperty, new TrayIcons { _trayIcon });
    }

    private void OpenGallery()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_galleryWindow == null || !_galleryWindow.IsVisible)
            {
                _galleryWindow = new WidgetGalleryWindow();
                _galleryWindow.WidgetAddRequested += OnWidgetAddRequested;
                _galleryWindow.Closed += (_, _) => _galleryWindow = null;
                _galleryWindow.Show();
            }
            else
            {
                _galleryWindow.Activate();
                _galleryWindow.WindowState = WindowState.Normal;
            }
        });
    }

    private void OpenSettings()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_settingsWindow == null || !_settingsWindow.IsVisible)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Closed += (_, _) => _settingsWindow = null;
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.Activate();
                _settingsWindow.WindowState = WindowState.Normal;
            }
        });
    }

    private void OnWidgetAddRequested(string type)
    {
        int x = _nextWidgetOffset;
        int y = 60 + new Random().Next(0, 200);
        _nextWidgetOffset += 320;

        var window = CreateWidgetWindow(type, x, y, 280, 200);
        window.Show();
        SaveLayout();
    }

    private void ExitApp()
    {
        SaveLayout();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}
