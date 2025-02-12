using System;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RemoteMouse.Discovery.Http.Contracts;
using RemoteMouse.Discovery.Ssdp.Contracts;
using RemoteMouse.Network;
using Rssdp;

namespace RemoteMouse.ViewModels.Desktop;

// Here all the activities related to device publication, server handling will take place.
public partial class DashboardViewModel() : ViewModelBase, IActivatableViewModel
{
    private readonly IDevicePublisher _devicePublisher = null!;
    private readonly IHttpServer _httpServer = null!;

    [Reactive] private string _connectivityStatus = "Disconnected";

    [Reactive] private SsdpRootDevice? _device;

    [Reactive] private DeviceNetwork? _deviceNetwork;

    public DashboardViewModel(IHttpServer httpServer, IDevicePublisher devicePublisher) : this()
    {
        _httpServer = httpServer;
        _devicePublisher = devicePublisher;

        this.WhenActivated(disposables => ObserveNetworkChanges().DisposeWith(disposables));
    }

    public ViewModelActivator Activator { get; } = new();

    private IDisposable ObserveNetworkChanges()
    {
        return NetworkStatus.OnNetworkChange
            // Set device network in internal state of view model
            .Select(SetDeviceNetwork)

            // Then publish device on that device network
            .Select(PublishDevice)

            // And remove the old device from the old network
            .Switch()

            // Then start the Http server
            .Select(StartHttpServer)

            // And stop listening on the previous network
            .Switch()

            // Then create appropriate Http request context handler (Websocket / Simple Http)
            .Select(_httpServer.CreateContextHandler)

            // And keep handling the Http request context using the above created context handler.
            .SelectMany(contextHandler => contextHandler.HandleContext(GetDeviceOrThrow()))

            // And keep notifying when 
            // 01. A new Http request arrives.
            // 02. A new Websocket request arrives.
            // 03. A new Websocket message arrives.
            .Subscribe(
                result => Console.WriteLine($"onNext: {result}"),
                exception => Console.WriteLine($"onError: {exception}"),
                () => Console.WriteLine("onCompleted: ✅")
            );
    }

    private DeviceNetwork? SetDeviceNetwork(DeviceNetwork? deviceNetwork = null)
    {
        Console.WriteLine($"Network Connection: {deviceNetwork?.LocalIpAddress}");

        DeviceNetwork = deviceNetwork;

        ConnectivityStatus = deviceNetwork is null ? "Disconnected" : "Connected";

        return deviceNetwork;
    }

    private IObservable<SsdpRootDevice?> PublishDevice(DeviceNetwork? deviceNetwork)
    {
        return deviceNetwork is null ? Observable.Return<SsdpRootDevice?>(null) : _devicePublisher.PublishDevice(deviceNetwork);
    }

    private IObservable<HttpListenerContext> StartHttpServer(SsdpRootDevice? device)
    {
        Device = device;

        return device is null ? Observable.Empty<HttpListenerContext>() : _httpServer.StartListening(device);
    }

    private SsdpRootDevice GetDeviceOrThrow()
    {
        return Device ?? throw new NullReferenceException("Device not set. Publish the device before accessing");
    }
}