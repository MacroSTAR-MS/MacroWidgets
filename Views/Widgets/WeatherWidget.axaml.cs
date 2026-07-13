using Avalonia.Controls;
using MacWidget.ViewModels.Widgets;

namespace MacWidget.Views.Widgets;

public partial class WeatherWidget : UserControl
{
    public WeatherWidget()
    {
        InitializeComponent();
        DataContext = new WeatherViewModel();
    }
}
