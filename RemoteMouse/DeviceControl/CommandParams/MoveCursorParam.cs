using System.Diagnostics.CodeAnalysis;

namespace RemoteMouse.DeviceControl.CommandParams;

public struct MoveCursorParam
{
    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public double Dx { get; set; }

    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public double Dy { get; set; }
}