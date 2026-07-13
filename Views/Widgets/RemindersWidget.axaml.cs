using Avalonia.Controls;
using MacroWidgets.ViewModels.Widgets;

namespace MacroWidgets.Views.Widgets;

public partial class RemindersWidget : UserControl
{
    public RemindersWidget()
    {
        InitializeComponent();
        DataContext = new RemindersViewModel();
    }
}
