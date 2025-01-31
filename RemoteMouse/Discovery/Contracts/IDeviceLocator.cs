using System;
using Rssdp;

namespace RemoteMouse.Discovery.Contracts;

public interface IDeviceLocator
{
    public IObservable<SsdpDevice[]> LocateDevices();
}