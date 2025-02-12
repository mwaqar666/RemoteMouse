using Avalonia.ReactiveUI;
using RemoteMouse.ViewModels.Desktop;

namespace RemoteMouse.Views.Desktop;

public partial class MainWindowView : ReactiveWindow<MainWindowViewModel>
{
    public MainWindowView()
    {
        InitializeComponent();
    }
}