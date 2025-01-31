using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DI;
using RemoteMouse.ViewModels.Desktop;
using RemoteMouse.ViewModels.Mobile;
using RemoteMouse.Views.Desktop;
using RemoteMouse.Views.Mobile;

namespace RemoteMouse;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Initialize the dependency service container
        var collection = new ServiceCollection();

        collection.AddCommonServices();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktopApp:
            {
                collection.AddDesktopServices();

                desktopApp.MainWindow = new MainWindow
                {
                    DataContext = collection.BuildServiceProvider().GetRequiredService<DashboardViewModel>()
                };

                break;
            }

            case ISingleViewApplicationLifetime mobileApp:
            {
                collection.AddMobileServices();

                mobileApp.MainView = new LocateDeviceView
                {
                    DataContext = collection.BuildServiceProvider().GetRequiredService<LocateDeviceViewModel>()
                };

                break;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}