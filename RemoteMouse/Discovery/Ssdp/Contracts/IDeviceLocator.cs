using System;
using Rssdp;

namespace RemoteMouse.Discovery.Ssdp.Contracts;

public interface IDeviceLocator
{
    public IObservable<SsdpDevice[]> LocateDevices();
}