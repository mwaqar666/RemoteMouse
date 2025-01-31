using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
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
            resourceFactory.GetResources<SsdpDevicePublisher, DescriptionHost>,
            scopedResource => Observable.Create<SsdpDevice>(observer =>
            {
                var (devicePublisher, descriptionHost) = scopedResource.Resources;

                var device = CreateDevice(deviceNetwork);

                devicePublisher.AddDevice(device);

                var subscription = descriptionHost
                    .ServeDeviceDocument(device)
                    .Subscribe(
                        _ => Debug.WriteLine($"Http listener started"),
                        observer.OnError,
                        observer.OnCompleted
                    );

                observer.OnNext(device);

                observer.OnCompleted();

                return () =>
                {
                    subscription.Dispose();

                    devicePublisher.RemoveDevice(device);
                };
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
            DeviceType = GetDeviceType(), // Device type
            FriendlyName = "Remote Mouse Desktop", // Name of the device
            DeviceVersion = 1, // Version of device
            Manufacturer = Environment.MachineName,
            ModelName = Environment.MachineName
        };
    }

    private static string GetDeviceType()
    {
        var applicationType = GetApplicationType();

        return $"{Constants.ApplicationAuthor}-{applicationType.ToString().ToLower()}";
    }

    private static string CreateDeviceDescriptionUrl(DeviceNetwork deviceNetwork)
    {
        return new StringBuilder().Append("http://").Append(deviceNetwork.LocalIpAddress).Append("/device-description.xml").ToString();
    }

    private static ApplicationType GetApplicationType()
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) return ApplicationType.Desktop;

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return ApplicationType.Mobile;

        throw new Exception($"Unknown operating system: {Environment.OSVersion}");
    }
}