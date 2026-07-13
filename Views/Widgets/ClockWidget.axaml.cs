using Avalonia.Controls;
using MacWidget.ViewModels.Widgets;

namespace MacWidget.Views.Widgets;

public partial class ClockWidget : UserControl
{
    public ClockWidget()
    {
        InitializeComponent();
        DataContext = new ClockViewModel();
    }
}
