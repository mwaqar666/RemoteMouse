using Avalonia.ReactiveUI;
using RemoteMouse.ViewModels.Desktop;

namespace RemoteMouse.Views.Desktop;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
    }
}