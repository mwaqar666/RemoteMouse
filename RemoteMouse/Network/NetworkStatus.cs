using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace RemoteMouse.Network;

public static class NetworkStatus
{
    public static IObservable<DeviceNetwork?> NetworkChange { get; } = FromAvailabilityChanged()
        .Merge(FromAddressChanged())
        .DistinctUntilChanged()
        .StartWith(GetDeviceNetwork());

    private static IObservable<DeviceNetwork?> FromAvailabilityChanged()
    {
        return Observable
            .FromEventPattern<NetworkAvailabilityChangedEventHandler, NetworkAvailabilityEventArgs>(
                handler => System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += handler,
                handler => System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged -= handler
            )
            .Throttle(TimeSpan.FromSeconds(2))
            .DistinctUntilChanged(pattern => pattern.EventArgs.IsAvailable)
            .Select(_ => GetDeviceNetwork());
    }

    private static IObservable<DeviceNetwork?> FromAddressChanged()
    {
        return Observable
            .FromEventPattern<NetworkAddressChangedEventHandler, EventArgs>(
                handler => System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += handler,
                handler => System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged -= handler
            )
            .Throttle(TimeSpan.FromSeconds(2))
            .Select(_ => GetDeviceNetwork());
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