using System;
using RemoteMouse.Network;
using Rssdp;

namespace RemoteMouse.Discovery.Ssdp.Contracts;

public interface IDevicePublisher
{
    public IObservable<SsdpRootDevice> PublishDevice(DeviceNetwork deviceNetwork);
}