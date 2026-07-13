using Avalonia.Controls;
using MacWidget.ViewModels.Widgets;

namespace MacWidget.Views.Widgets;

public partial class CalendarWidget : UserControl
{
    public CalendarWidget()
    {
        InitializeComponent();
        DataContext = new CalendarViewModel();
    }
}
