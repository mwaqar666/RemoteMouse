using System.Diagnostics.CodeAnalysis;

namespace RemoteMouse.DeviceControl.CommandParams;

public enum ClickButton
{
    LeftClick,
    RightClick
}

public struct MouseClickParam
{
    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public ClickButton ClickButton { get; set; }
}