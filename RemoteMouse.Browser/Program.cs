﻿using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;

namespace RemoteMouse.Browser;

internal sealed class Program
{
    private static Task Main(string[] args)
    {
        return BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>();
    }
}