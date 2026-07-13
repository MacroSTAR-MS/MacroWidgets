using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MacroWidgets.Views;

namespace MacroWidgets.Services;

/// <summary>
/// 持久化服务：保存和恢复小组件布局。
/// 配置文件保存在 AppData/MacroWidgets/config.json
/// </summary>
public class WidgetPersistenceService
{
    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MacroWidgets");

    private static readonly string ConfigPath = Path.Combine(ConfigDir, "config.json");

    private static readonly string SettingsPath = Path.Combine(ConfigDir, "settings.json");

    public MacroWidgetsConfig Config { get; private set; } = new();

    public MacroWidgetsSettings Settings { get; private set; } = new();

    /// <summary>
    /// 加载配置。如果文件不存在则使用默认值。
    /// </summary>
    public void Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                Config = JsonSerializer.Deserialize<MacroWidgetsConfig>(json) ?? new MacroWidgetsConfig();
            }

            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                Settings = JsonSerializer.Deserialize<MacroWidgetsSettings>(json) ?? new MacroWidgetsSettings();
            }
        }
        catch (Exception)
        {
            // 配置文件损坏，使用默认值
            Config = new MacroWidgetsConfig();
            Settings = new MacroWidgetsSettings();
        }
    }

    /// <summary>
    /// 保存小组件布局配置
    /// </summary>
    public void SaveLayout(List<WidgetLayoutInfo> widgets)
    {
        Config.Widgets = widgets;
        Save(ConfigPath, Config);
    }

    /// <summary>
    /// 保存应用设置
    /// </summary>
    public void SaveSettings()
    {
        Save(SettingsPath, Settings);
    }

    private static void Save(string path, object data)
    {
        try
        {
            Directory.CreateDirectory(ConfigDir);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(path, json);
        }
        catch (Exception)
        {
            // 静默失败
        }
    }
}

/// <summary>
/// 应用配置：小组件布局
/// </summary>
public class MacroWidgetsConfig
{
    public List<WidgetLayoutInfo> Widgets { get; set; } = new();
}

/// <summary>
/// 应用设置（埋点，后续扩展）
/// </summary>
public class MacroWidgetsSettings
{
    /// <summary>天气城市</summary>
    public string WeatherCity { get; set; } = "广州";

    /// <summary>开机自启</summary>
    public bool AutoStart { get; set; } = false;

    /// <summary>主题 (Dark/Light/System)</summary>
    public string Theme { get; set; } = "Dark";

    /// <summary>通知开关</summary>
    public bool EnableNotifications { get; set; } = true;

    /// <summary>语言</summary>
    public string Language { get; set; } = "zh-CN";

    /// <summary>天气 API Key（埋点）</summary>
    public string? WeatherApiKey { get; set; }

    /// <summary>更新检查间隔（天）</summary>
    public int UpdateCheckIntervalDays { get; set; } = 7;
}
