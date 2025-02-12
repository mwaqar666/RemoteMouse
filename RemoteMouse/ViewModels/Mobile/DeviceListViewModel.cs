using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Rssdp;

namespace RemoteMouse.ViewModels.Mobile;

public class DeviceListViewModel
{
    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public ObservableCollection<SsdpDevice> Devices { get; } = [];

    public void AddDevice(SsdpDevice device)
    {
        Devices.Add(device);
    }

    public void ClearDevices()
    {
        Devices.Clear();
    }

    public void AddDevices(SsdpDevice[] devices)
    {
        foreach (var device in devices) AddDevice(device);
    }
}