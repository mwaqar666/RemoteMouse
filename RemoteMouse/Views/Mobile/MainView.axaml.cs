using Avalonia.ReactiveUI;
using RemoteMouse.ViewModels.Mobile;

namespace RemoteMouse.Views.Mobile;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
}