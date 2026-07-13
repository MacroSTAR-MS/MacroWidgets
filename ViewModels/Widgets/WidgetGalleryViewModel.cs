using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MacWidget.Services;

namespace MacWidget.ViewModels.Widgets;

public partial class WidgetGalleryViewModel : ObservableObject
{
    public List<WidgetRegistryEntry> AvailableWidgets => WidgetManager.Instance.AvailableWidgets;

    /// <summary>
    /// 当用户点击添加小组件时触发。
    /// 参数：widget type (如 "clock", "weather" 等)
    /// </summary>
    public event Action<string>? WidgetAddRequested;

    /// <summary>
    /// 当用户点击关闭窗口时触发。
    /// </summary>
    public event Action? CloseRequested;

    [RelayCommand]
    private void AddWidget(string type)
    {
        WidgetAddRequested?.Invoke(type);
    }

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke();
    }
}
