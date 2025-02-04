using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RemoteMouse.Discovery.Contracts;
using RemoteMouse.Discovery.Publisher;
using RemoteMouse.Network;
using Rssdp;

namespace RemoteMouse.ViewModels.Desktop;

public partial class DashboardViewModel() : ViewModelBase, IActivatableViewModel
{
    [Reactive] private string _connectivityStatus = "Disconnected";

    [Reactive] private DeviceNetwork? _deviceNetwork;

    public DashboardViewModel(IDevicePublisher devicePublisher, DescriptionHost descriptionHost) : this()
    {
        this.WhenActivated(disposables => ObserveNetworkChanges(devicePublisher, descriptionHost).DisposeWith(disposables));
    }

    public ViewModelActivator Activator { get; } = new();

    private void SetDeviceNetwork(DeviceNetwork? deviceNetwork = null)
    {
        DeviceNetwork = deviceNetwork;

        ConnectivityStatus = deviceNetwork is null ? "Disconnected" : "Connected";
    }

    private IDisposable ObserveNetworkChanges(IDevicePublisher devicePublisher, DescriptionHost descriptionHost)
    {
        return NetworkStatus.NetworkChange
            .Do(SetDeviceNetwork)
            .Select(deviceNetwork => deviceNetwork is null ? Observable.Empty<SsdpDevice>() : devicePublisher.PublishDevice(deviceNetwork))
            .Switch()
            .Select(descriptionHost.ServeDeviceDocument)
            .Switch()
            .Subscribe(
                x => Debug.WriteLine($"Device published on IP: {x}"),
                x => Debug.WriteLine($"onError: {x}"),
                () => Debug.WriteLine("onCompleted")
            );
    }
}