using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RemoteMouse.Discovery.Contracts;
using RemoteMouse.Discovery.Publisher;
using RemoteMouse.Network;

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
        var networkChange = NetworkStatus.NetworkChange;

        return networkChange
            .Do(SetDeviceNetwork)
            .Select(deviceNetwork =>
            {
                if (deviceNetwork is null) return Observable.Empty<Unit>();

                return devicePublisher.PublishDevice(deviceNetwork)
                    .SelectMany(descriptionHost.ServeDeviceDocument)
                    .TakeUntil(networkChange); // Unsubscribe when firstObservable emits again
            })
            .Subscribe();
    }
}