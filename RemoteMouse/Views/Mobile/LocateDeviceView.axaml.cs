using Avalonia.ReactiveUI;
using RemoteMouse.ViewModels.Mobile;

namespace RemoteMouse.Views.Mobile;

public partial class LocateDeviceView : ReactiveUserControl<LocateDeviceViewModel>
{
    public LocateDeviceView()
    {
        InitializeComponent();
    }
}