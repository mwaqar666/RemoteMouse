using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RemoteMouse.Discovery.Ssdp.Contracts;
using Rssdp;

namespace RemoteMouse.ViewModels.Mobile;

public partial class LocateDeviceViewModel() : ViewModelBase, IActivatableViewModel
{
    [Reactive] private DeviceListViewModel _deviceListViewModel = new();

    [Reactive] private bool _isSearching;

    public LocateDeviceViewModel(IDeviceLocator deviceLocator) : this()
    {
        SearchForDevicesCommand = ReactiveCommand.CreateRunInBackground(() => LocateDevices(deviceLocator));

        this.WhenActivated(
            disposables => LocateDevices(deviceLocator).DisposeWith(disposables)
        );
    }

    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public required ICommand SearchForDevicesCommand { get; set; }

    public ViewModelActivator Activator { get; } = new();

    private IDisposable LocateDevices(IDeviceLocator deviceLocator)
    {
        IsSearching = true;

        DeviceListViewModel.ClearDevices();

        return deviceLocator.LocateDevices().Subscribe(AddDiscoveredDevice);
    }

    private void AddDiscoveredDevice(SsdpDevice[] devices)
    {
        IsSearching = false;

        DeviceListViewModel.AddDevices(devices);
    }
}