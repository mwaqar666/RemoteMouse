using System;
using System.Reactive.Linq;
using RemoteMouse.DI.Contracts;
using RemoteMouse.Discovery.Ssdp.Contracts;
using RemoteMouse.Models;
using RemoteMouse.Network;
using Rssdp;

namespace RemoteMouse.Discovery.Ssdp;

public class DevicePublisher(IResourceFactory resourceFactory) : IDevicePublisher
{
    public IObservable<SsdpRootDevice> PublishDevice(DeviceNetwork deviceNetwork)
    {
        return Observable.Using(
            resourceFactory.GetResource<SsdpDevicePublisher>,
            resourceContainer => Observable.Create<SsdpRootDevice>(observer =>
            {
                var devicePublisher = resourceContainer.Resources;

                devicePublisher.NotificationBroadcastInterval = TimeSpan.FromSeconds(5);

                var device = Device.CreateFrom(deviceNetwork.LocalIpAddress);

                Console.WriteLine($"Adding device: {device}");

                devicePublisher.AddDevice(device);

                observer.OnNext(device);

                return () =>
                {
                    Console.WriteLine($"Removing device: {device}");

                    devicePublisher.RemoveDevice(device);
                };
            })
        );
    }
}