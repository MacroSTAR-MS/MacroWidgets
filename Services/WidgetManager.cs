using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using MacroWidgets.Views.Widgets;
using MacroWidgets.Views.Gallery;

namespace MacroWidgets.Services;

public class WidgetManager
{
    public static WidgetManager Instance { get; } = new();

    public ObservableCollection<WidgetConfig> ActiveWidgets { get; } = new();

    // Widget type registry
    public List<WidgetRegistryEntry> AvailableWidgets { get; } = new()
    {
        new("时钟", "显示当前时间和日期", "clock"),
        new("天气", "查看当前天气和逐时预报", "weather"),
        new("日历", "月历视图，高亮今日", "calendar"),
        new("提醒事项", "待办任务清单", "reminders"),
    };

    public UserControl CreateWidget(string type)
    {
        return type switch
        {
            "clock" => new ClockWidget(),
            "weather" => new WeatherWidget(),
            "calendar" => new CalendarWidget(),
            "reminders" => new RemindersWidget(),
            _ => throw new ArgumentException($"Unknown widget type: {type}")
        };
    }

    public void AddWidget(string type, double x, double y, WidgetSize size)
    {
        var config = new WidgetConfig(type, x, y, size);
        ActiveWidgets.Add(config);
    }

    public void RemoveWidget(WidgetConfig config)
    {
        ActiveWidgets.Remove(config);
    }
}

public record WidgetRegistryEntry(string Name, string Description, string Type);

public enum WidgetSize { Small, Medium, Large }

public class WidgetConfig
{
    public string Type { get; }
    public double X { get; set; }
    public double Y { get; set; }
    public WidgetSize Size { get; set; }
    public UserControl? WidgetInstance { get; set; }

    public WidgetConfig(string type, double x, double y, WidgetSize size)
    {
        Type = type;
        X = x;
        Y = y;
        Size = size;
    }
}
