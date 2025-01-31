using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RemoteMouse.Discovery.Contracts;
using RemoteMouse.Network;

namespace RemoteMouse.ViewModels.Desktop;

public partial class DashboardViewModel() : ViewModelBase, IActivatableViewModel
{
    private readonly IDevicePublisher _devicePublisher = null!;

    [Reactive] private string _connectivityStatus = "Disconnected";

    [Reactive] private DeviceNetwork? _deviceNetwork;

    private IDisposable? _subscription;

    public DashboardViewModel(IDevicePublisher devicePublisher) : this()
    {
        _devicePublisher = devicePublisher;

        this.WhenActivated(disposables =>
        {
            NetworkStatus.NetworkChange
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(PublishDeviceOnNetworkChange, LogDevicePublicationException)
                .DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; } = new();

    private void PublishDeviceOnNetworkChange(DeviceNetwork? deviceNetwork)
    {
        SetDeviceNetwork(deviceNetwork);

        if (DeviceNetwork is null)
        {
            _subscription?.Dispose();

            return;
        }

        _subscription = _devicePublisher.PublishDevice(DeviceNetwork).ObserveOn(RxApp.TaskpoolScheduler).Subscribe();
    }

    private void LogDevicePublicationException(Exception exception)
    {
        SetDeviceNetwork();

        Debug.WriteLine($"Exception: {exception}");
    }

    private void SetDeviceNetwork(DeviceNetwork? deviceNetwork = null)
    {
        DeviceNetwork = deviceNetwork;

        ConnectivityStatus = deviceNetwork is null ? "Disconnected" : "Connected";
    }
}