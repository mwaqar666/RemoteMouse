using System;
using System.Net;
using Rssdp;

namespace RemoteMouse.Models;

public static class Device
{
    /// <summary>Create a new device from IP address</summary>
    /// <param name="ipAddress">IP address where the device will be created</param>
    /// <returns>The newly created SSDP device</returns>
    public static SsdpRootDevice CreateFrom(IPAddress ipAddress)
    {
        return new SsdpRootDevice
        {
            Uuid = Guid.NewGuid().ToString(),
            CacheLifetime = TimeSpan.FromMinutes(30), // How long the device is cached in other systems
            Location = new Uri($"http://{ipAddress}/device-description.xml"), // URL for the device description
            PresentationUrl = new Uri($"http://{ipAddress}/device-description.xml"), // URL for the device description
            DeviceTypeNamespace = "schemas-upnp-org", // Custom namespace
            DeviceType = "RemoteMouseDesktop", // Device type
            FriendlyName = "Remote Mouse Desktop", // Name of the device
            DeviceVersion = 1, // Version of device
            Manufacturer = Environment.MachineName,
            ModelName = Environment.MachineName
        };
    }
}