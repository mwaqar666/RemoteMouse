using System;
using RemoteMouse.Network;
using Rssdp;

namespace RemoteMouse.Discovery.Contracts;

public interface IDevicePublisher
{
    public IObservable<SsdpDevice> PublishDevice(DeviceNetwork deviceNetwork);
}