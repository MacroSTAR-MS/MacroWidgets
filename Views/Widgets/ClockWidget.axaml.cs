using Avalonia.Controls;
using MacroWidgets.ViewModels.Widgets;

namespace MacroWidgets.Views.Widgets;

public partial class ClockWidget : UserControl
{
    public ClockWidget()
    {
        InitializeComponent();
        DataContext = new ClockViewModel();
    }
}
