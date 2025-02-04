using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RemoteMouse.Discovery.Contracts;
using Rssdp;

namespace RemoteMouse.ViewModels.Mobile;

public partial class LocateDeviceViewModel() : ViewModelBase, IActivatableViewModel
{
    [Reactive] private DeviceListViewModel? _deviceListViewModel;

    [Reactive] private bool _isSearching;

    public LocateDeviceViewModel(IDeviceLocator deviceLocator) : this()
    {
        SearchForDevicesCommand = ReactiveCommand.CreateRunInBackground(() => LocateDevices(deviceLocator));

        this.WhenActivated(disposables => LocateDevices(deviceLocator).DisposeWith(disposables));
    }

    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public required ICommand SearchForDevicesCommand { get; set; }

    public ViewModelActivator Activator { get; } = new();

    private IDisposable LocateDevices(IDeviceLocator deviceLocator)
    {
        IsSearching = true;

        return deviceLocator.LocateDevices().Subscribe(SetDevices);
    }

    private void SetDevices(SsdpDevice[] devices)
    {
        DeviceListViewModel = new DeviceListViewModel
        {
            Devices = devices
        };

        IsSearching = false;
    }
}