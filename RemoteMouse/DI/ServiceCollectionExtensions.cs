using System.Net;
using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DeviceControl;
using RemoteMouse.DeviceControl.Platforms;
using RemoteMouse.DI.Contracts;
using RemoteMouse.Discovery.Http;
using RemoteMouse.Discovery.Http.Context;
using RemoteMouse.Discovery.Http.Contracts;
using RemoteMouse.Discovery.Ssdp;
using RemoteMouse.Discovery.Ssdp.Contracts;
using RemoteMouse.ViewModels.Desktop;
using RemoteMouse.ViewModels.Mobile;
using Rssdp;

namespace RemoteMouse.DI;

public static class ServiceCollectionExtensions
{
    public static void AddCommonDependencies(this IServiceCollection collection)
    {
        collection.AddTransient<IResourceFactory, ResourceFactory>();
    }

    public static void AddDesktopDependencies(this IServiceCollection collection)
    {
        collection.AddDesktopViewModels();

        collection.AddDesktopServices();
    }

    public static void AddMobileDependencies(this IServiceCollection collection)
    {
        collection.AddMobileViewModels();

        collection.AddMobileServices();
    }

    private static void AddDesktopViewModels(this IServiceCollection collection)
    {
        // Root View Model
        collection.AddSingleton<MainWindowViewModel>();

        collection.AddTransient<DashboardViewModel>();
    }

    private static void AddDesktopServices(this IServiceCollection collection)
    {
        collection.AddHttpServices();

        collection.AddSsdpPublisherServices();

        collection.AddDeviceControlServices();
    }

    private static void AddMobileViewModels(this IServiceCollection collection)
    {
        // Root View Model
        collection.AddSingleton<MainViewModel>();

        collection.AddTransient<LocateDeviceViewModel>();
    }

    private static void AddMobileServices(this IServiceCollection collection)
    {
        collection.AddSsdpLocatorServices();
    }

    private static void AddHttpServices(this IServiceCollection collection)
    {
        collection.AddTransient<HttpListener>();
        collection.AddTransient<IHttpServer, HttpServer>();

        collection.AddTransient<HttpContextHandler>();
        collection.AddTransient<WebSocketContextHandler>();
    }

    private static void AddSsdpLocatorServices(this IServiceCollection collection)
    {
        collection.AddTransient<SsdpDeviceLocator>();
        collection.AddTransient<IDeviceLocator, DeviceLocator>();
    }

    private static void AddSsdpPublisherServices(this IServiceCollection collection)
    {
        collection.AddTransient<SsdpDevicePublisher>();
        collection.AddTransient<IDevicePublisher, DevicePublisher>();
    }

    private static void AddDeviceControlServices(this IServiceCollection collection)
    {
        collection.AddTransient<CommandExecutor>();

        collection.AddTransient<WindowsCommandHandler>();
        collection.AddTransient<LinuxCommandHandler>();
        collection.AddTransient<MacOSCommandHandler>();
    }
}