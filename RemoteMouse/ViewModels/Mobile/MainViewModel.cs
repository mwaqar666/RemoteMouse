using System;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.SourceGenerators;

namespace RemoteMouse.ViewModels.Mobile;

public partial class MainViewModel(IServiceProvider serviceProvider) : ViewModelBase
{
    [Reactive] private LocateDeviceViewModel _locateDeviceViewModel = serviceProvider.GetRequiredService<LocateDeviceViewModel>();
}