using System;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.SourceGenerators;

namespace RemoteMouse.ViewModels.Desktop;

public partial class MainWindowViewModel(IServiceProvider serviceProvider) : ViewModelBase
{
    [Reactive] private DashboardViewModel _dashboardViewModel = serviceProvider.GetRequiredService<DashboardViewModel>();
}