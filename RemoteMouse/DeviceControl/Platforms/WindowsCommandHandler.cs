using System;
using RemoteMouse.DeviceControl.CommandParams;
using RemoteMouse.DeviceControl.Contracts;

namespace RemoteMouse.DeviceControl.Platforms;

public class WindowsCommandHandler : ICommandHandler
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