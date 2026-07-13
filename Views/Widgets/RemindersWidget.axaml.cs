using Avalonia.Controls;
using MacWidget.ViewModels.Widgets;

namespace MacWidget.Views.Widgets;

public partial class RemindersWidget : UserControl
{
    public RemindersWidget()
    {
        InitializeComponent();
        DataContext = new RemindersViewModel();
    }
}
