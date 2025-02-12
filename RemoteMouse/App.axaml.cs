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

        collection.AddCommonDependencies();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktopApp:
            {
                RunDesktopVersion(collection, desktopApp);

                break;
            }

            case ISingleViewApplicationLifetime mobileApp:
            {
                RunMobileVersion(collection, mobileApp);

                break;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void RunDesktopVersion(IServiceCollection collection, IClassicDesktopStyleApplicationLifetime app)
    {
        collection.AddDesktopDependencies();

        var serviceProvider = collection.BuildServiceProvider();

        app.MainWindow = new MainWindowView
        {
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
        };
    }

    private static void RunMobileVersion(IServiceCollection collection, ISingleViewApplicationLifetime app)
    {
        collection.AddMobileDependencies();

        var serviceProvider = collection.BuildServiceProvider();

        app.MainView = new MainView
        {
            DataContext = serviceProvider.GetRequiredService<MainViewModel>()
        };
    }
}