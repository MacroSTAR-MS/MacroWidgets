using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using MacWidget.ViewModels.Widgets;

namespace MacWidget.Views.Gallery;

public partial class WidgetGalleryWindow : Window
{
    private readonly WidgetGalleryViewModel _viewModel;

    public WidgetGalleryWindow()
    {
        InitializeComponent();
        _viewModel = new WidgetGalleryViewModel();
        DataContext = _viewModel;

        // ViewModel 事件 → 窗口操作
        _viewModel.WidgetAddRequested += OnWidgetAddRequested;
        _viewModel.CloseRequested += () => Close();

        // 标题栏拖动
        var titleBar = this.FindControl<Border>("TitleBar");
        if (titleBar != null)
        {
            titleBar.PointerPressed += OnTitleBarPointerPressed;
        }
    }

    /// <summary>
    /// 当用户在画廊中点击添加小组件时触发。
    /// App 订阅此事件来创建新的桌面小组件窗口。
    /// </summary>
    public event Action<string>? WidgetAddRequested;

    private void OnWidgetAddRequested(string type)
    {
        WidgetAddRequested?.Invoke(type);
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }
}
