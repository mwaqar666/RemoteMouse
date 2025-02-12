using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace RemoteMouse.Network;

public static class NetworkStatus
{
    public static IObservable<DeviceNetwork?> OnNetworkChange { get; } = FromAvailabilityChanged()
        .Concat(FromAddressChanged())
        .DistinctUntilChanged()
        .StartWith(GetDeviceNetwork());

    private static IObservable<DeviceNetwork?> FromAvailabilityChanged()
    {
        return Observable
            .FromEventPattern<NetworkAvailabilityChangedEventHandler, NetworkAvailabilityEventArgs>(
                handler => NetworkChange.NetworkAvailabilityChanged += handler,
                handler => NetworkChange.NetworkAvailabilityChanged -= handler
            )
            .Throttle(TimeSpan.FromSeconds(2))
            .Select(_ => GetDeviceNetwork())
            .DistinctUntilChanged();
    }

    private static IObservable<DeviceNetwork?> FromAddressChanged()
    {
        return Observable
            .FromEventPattern<NetworkAddressChangedEventHandler, EventArgs>(
                handler => NetworkChange.NetworkAddressChanged += handler,
                handler => NetworkChange.NetworkAddressChanged -= handler
            )
            .Throttle(TimeSpan.FromSeconds(2))
            .Select(_ => GetDeviceNetwork())
            .DistinctUntilChanged();
    }

    private static DeviceNetwork? GetDeviceNetwork()
    {
        var connectedNetwork = GetConnectedNetworkInterface();

        if (connectedNetwork is null) return null;

        var localIpAddress = GetLocalIpAddress(connectedNetwork);

        return localIpAddress is not null ? new DeviceNetwork(connectedNetwork, localIpAddress) : null;
    }

    private static NetworkInterface? GetConnectedNetworkInterface()
    {
        return NetworkInterface
            .GetAllNetworkInterfaces()
            .FirstOrDefault(x => x.OperationalStatus is OperationalStatus.Up && x.NetworkInterfaceType is NetworkInterfaceType.Ethernet or NetworkInterfaceType.Wireless80211);
    }

    private static IPAddress? GetLocalIpAddress(NetworkInterface networkInterface)
    {
        return networkInterface
            .GetIPProperties()
            .UnicastAddresses
            .Where(x => x.Address.AddressFamily is AddressFamily.InterNetwork)
            .Select(x => x.Address)
            .FirstOrDefault();
    }
}