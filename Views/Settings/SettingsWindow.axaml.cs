using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using MacroWidgets.Views.Settings.Pages;

namespace MacroWidgets.Views.Settings;

public partial class SettingsWindow : Window
{
    private readonly Border[] _navItems;
    private int _selectedIndex = -1;
    private Border? _hoveredItem;

    public SettingsWindow()
    {
        InitializeComponent();

        _navItems = new[]
        {
            this.FindControl<Border>("NavGeneral")!,
            this.FindControl<Border>("NavAppearance")!,
            this.FindControl<Border>("NavNotification")!,
            this.FindControl<Border>("NavAdvanced")!,
        };

        NavigateTo(0);
    }

    private void NavigateTo(int index)
    {
        // 更新选中态
        for (int i = 0; i < _navItems.Length; i++)
        {
            if (i == index)
                _navItems[i].Background = (IBrush?)Application.Current.FindResource("NavItemSelected");
            else
                _navItems[i].Background = Brushes.Transparent;
        }
        _selectedIndex = index;

        // 切换右侧内容
        ContentArea.Content = index switch
        {
            0 => new GeneralSettingsPage(),
            1 => new AppearanceSettingsPage(),
            2 => new NotificationSettingsPage(),
            3 => new AdvancedSettingsPage(),
            _ => new GeneralSettingsPage(),
        };
    }

    // 导航项悬停效果
    private void Nav_PointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Border b && b != _navItems[_selectedIndex])
        {
            b.Background = (IBrush?)Application.Current.FindResource("NavItemHover");
            _hoveredItem = b;
        }
    }

    private void Nav_PointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is Border b && b != _navItems[_selectedIndex])
        {
            b.Background = Brushes.Transparent;
            if (_hoveredItem == b) _hoveredItem = null;
        }
    }

    private void NavGeneral_PointerPressed(object? sender, PointerPressedEventArgs e) => NavigateTo(0);
    private void NavAppearance_PointerPressed(object? sender, PointerPressedEventArgs e) => NavigateTo(1);
    private void NavNotification_PointerPressed(object? sender, PointerPressedEventArgs e) => NavigateTo(2);
    private void NavAdvanced_PointerPressed(object? sender, PointerPressedEventArgs e) => NavigateTo(3);
}
