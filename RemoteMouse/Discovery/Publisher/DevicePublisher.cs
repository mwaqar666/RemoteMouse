using System;
using System.Reactive.Linq;
using RemoteMouse.DI.Contracts;
using RemoteMouse.Discovery.Contracts;
using RemoteMouse.Discovery.Enums;
using RemoteMouse.Network;
using Rssdp;

namespace RemoteMouse.Discovery.Publisher;

public class DevicePublisher(IScopedResourceFactory resourceFactory) : IDevicePublisher
{
    public IObservable<SsdpDevice> PublishDevice(DeviceNetwork deviceNetwork)
    {
        return Observable.Using(
            resourceFactory.GetResource<SsdpDevicePublisher>,
            resourceContainer => Observable.Create<SsdpDevice>(observer =>
            {
                var devicePublisher = resourceContainer.Resources;

                var device = CreateDevice(deviceNetwork);

                devicePublisher.AddDevice(device);

                observer.OnNext(device);

                observer.OnCompleted();

                return () => devicePublisher.RemoveDevice(device);
            })
        );
    }

    private static SsdpRootDevice CreateDevice(DeviceNetwork deviceNetwork)
    {
        return new SsdpRootDevice
        {
            Uuid = Guid.NewGuid().ToString(), // Unique identifier
            CacheLifetime = TimeSpan.FromDays(1), // How long the device is cached in other systems
            Location = new Uri(CreateDeviceDescriptionUrl(deviceNetwork)), // URL for the device description
            PresentationUrl = new Uri(CreateDeviceDescriptionUrl(deviceNetwork)),
            DeviceTypeNamespace = Constants.ApplicationAuthor, // Custom namespace
            DeviceType = CreateDeviceType(), // Device type
            FriendlyName = "Remote Mouse Desktop", // Name of the device
            DeviceVersion = 1, // Version of device
            Manufacturer = Environment.MachineName,
            ModelName = Environment.MachineName
        };
    }

    private static string CreateDeviceType()
    {
        return $"{Constants.ApplicationAuthor}-{GetApplicationType().ToString().ToLower()}";
    }

    private static string CreateDeviceDescriptionUrl(DeviceNetwork deviceNetwork)
    {
        return $"http://{deviceNetwork.LocalIpAddress}/device-description.xml";
    }

    private static ApplicationType GetApplicationType()
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) return ApplicationType.Desktop;

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return ApplicationType.Mobile;

        throw new Exception($"Unknown operating system: {Environment.OSVersion}");
    }
}