using System.Diagnostics.CodeAnalysis;
using Rssdp;

namespace RemoteMouse.ViewModels.Mobile;

public class DeviceListViewModel
{
    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public required SsdpDevice[] Devices { get; init; } = [];
}