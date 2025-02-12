using RemoteMouse.DeviceControl.CommandParams;

namespace RemoteMouse.DeviceControl.Contracts;

public interface ICommandHandler
{
    public void Execute(MoveCursorParam param);

    public void Execute(MouseClickParam param);
}