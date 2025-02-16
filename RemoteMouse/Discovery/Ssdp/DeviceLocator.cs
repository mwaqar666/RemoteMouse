using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using RemoteMouse.DI.Contracts;
using RemoteMouse.Discovery.Ssdp.Contracts;
using Rssdp;

namespace RemoteMouse.Discovery.Ssdp;

internal sealed class DeviceLocator(IResourceFactory resourceFactory) : IDeviceLocator
{
    public IObservable<SsdpDevice[]> LocateDevices()
    {
        return Observable.Using(
            resourceFactory.GetResource<SsdpDeviceLocator>,
            resourceContainer =>
                SearchForDevicesAsync(resourceContainer.Resources)
                    .SelectMany(FlattenDiscoveredDevices)
                    .SelectMany(GetEachDiscoveredDeviceInfo)
                    .ToArray()
        );
    }

    private static IObservable<IEnumerable<DiscoveredSsdpDevice>> SearchForDevicesAsync(SsdpDeviceLocator deviceLocator)
    {
        return Observable.FromAsync(() => deviceLocator.SearchAsync("urn:schemas-upnp-org:device:RemoteMouseDesktop:1", TimeSpan.FromSeconds(10)));
    }

    private static IEnumerable<DiscoveredSsdpDevice> FlattenDiscoveredDevices(IEnumerable<DiscoveredSsdpDevice> discoveredDevices)
    {
        return discoveredDevices;
    }

    private static IObservable<SsdpDevice> GetEachDiscoveredDeviceInfo(DiscoveredSsdpDevice discoveredDevice)
    {
        return Observable
            .FromAsync(discoveredDevice.GetDeviceInfo)
            .Catch((Exception exception) =>
            {
                Console.WriteLine($"Error getting device info for device {discoveredDevice.Usn}: {exception.Message}");

                return Observable.Empty<SsdpDevice>();
            });
    }
}