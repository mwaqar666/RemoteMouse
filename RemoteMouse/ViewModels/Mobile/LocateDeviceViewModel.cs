using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using RemoteMouse.Discovery.Contracts;
using Rssdp;

namespace RemoteMouse.ViewModels.Mobile;

public partial class LocateDeviceViewModel() : ViewModelBase, IActivatableViewModel
{
    [Reactive] private bool _isSearching;

    public LocateDeviceViewModel(IDeviceLocator deviceLocator) : this()
    {
        SearchForDevicesCommand = ReactiveCommand.CreateRunInBackground(deviceLocator.LocateDevices);
    }

    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public ObservableCollection<SsdpDevice> Devices { get; } = [];

    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public required ICommand SearchForDevicesCommand { get; init; }

    public ViewModelActivator Activator { get; } = new();

    private void OnFoundDevices(object? _, SsdpDevice[] devices)
    {
        Devices.Clear();

        foreach (var device in devices) Devices.Add(device);
    }

    private void OnSearchingForDevices(object? sender, bool isSearching)
    {
        IsSearching = isSearching;
    }
}