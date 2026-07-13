using Avalonia.Controls;
using MacroWidgets.ViewModels.Widgets;

namespace MacroWidgets.Views.Widgets;

public partial class CalendarWidget : UserControl
{
    public CalendarWidget()
    {
        InitializeComponent();
        DataContext = new CalendarViewModel();
    }
}
