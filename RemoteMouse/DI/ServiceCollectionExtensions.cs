using System.Net;
using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DI.Contracts;
using RemoteMouse.Discovery.Contracts;
using RemoteMouse.Discovery.Locator;
using RemoteMouse.Discovery.Publisher;
using RemoteMouse.ViewModels.Desktop;
using RemoteMouse.ViewModels.Mobile;
using Rssdp;

namespace RemoteMouse.DI;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<IScopedResourceFactory, ScopedResourceFactory>();
    }

    public static void AddDesktopServices(this IServiceCollection collection)
    {
        // View Models
        collection.AddTransient<DashboardViewModel>();

        // Services
        collection.AddTransient<HttpListener>();
        collection.AddTransient<DescriptionHost>();
        collection.AddTransient<SsdpDevicePublisher>();
        collection.AddTransient<IDevicePublisher, DevicePublisher>();
    }

    public static void AddMobileServices(this IServiceCollection collection)
    {
        // View Models
        collection.AddTransient<LocateDeviceViewModel>();

        // Services
        collection.AddTransient<SsdpDeviceLocator>();
        collection.AddTransient<IDeviceLocator, DeviceLocator>();
    }
}