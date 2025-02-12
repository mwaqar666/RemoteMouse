using System;
using RemoteMouse.DeviceControl.CommandParams;
using RemoteMouse.DeviceControl.Contracts;

namespace RemoteMouse.DeviceControl.Platforms;

// ReSharper disable once InconsistentNaming
public class MacOSCommandHandler : ICommandHandler
{
    public void Execute(MoveCursorParam param)
    {
        Console.WriteLine("MoveCursorCommand executed!");
    }

    public void Execute(MouseClickParam param)
    {
        Console.WriteLine("MouseClickCommand executed!");
    }
}