using Avalonia.ReactiveUI;
using RemoteMouse.ViewModels.Mobile;

namespace RemoteMouse.Views.Mobile;

public partial class DeviceListView : ReactiveUserControl<DeviceListViewModel>
{
    public DeviceListView()
    {
        InitializeComponent();
    }
}