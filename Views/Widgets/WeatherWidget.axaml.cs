using Avalonia.Controls;
using MacroWidgets.ViewModels.Widgets;

namespace MacroWidgets.Views.Widgets;

public partial class WeatherWidget : UserControl
{
    public WeatherWidget()
    {
        InitializeComponent();
        DataContext = new WeatherViewModel();
    }
}
