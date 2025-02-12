using System;
using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DeviceControl.CommandParams;
using RemoteMouse.DeviceControl.Contracts;
using RemoteMouse.DeviceControl.Platforms;

namespace RemoteMouse.DeviceControl;

public class CommandExecutor(IServiceProvider serviceProvider)
{
    private ICommandHandler? _commandHandler;

    private ICommandHandler CommandHandler => _commandHandler ?? throw new NullReferenceException("Command handler is null!");

    public void HandleCommand(object message)
    {
        GetCommandHandler().Execute(message);
    }

    private CommandExecutor GetCommandHandler()
    {
        if (OperatingSystem.IsWindows())
        {
            _commandHandler = serviceProvider.GetRequiredService<WindowsCommandHandler>();
            return this;
        }

        if (OperatingSystem.IsLinux())
        {
            _commandHandler = serviceProvider.GetRequiredService<LinuxCommandHandler>();
            return this;
        }

        // ReSharper disable once InvertIf
        if (OperatingSystem.IsMacOS())
        {
            _commandHandler = serviceProvider.GetRequiredService<MacOSCommandHandler>();
            return this;
        }

        throw new Exception("Unknown Operating System!");
    }

    private void Execute(object message)
    {
        switch (message)
        {
            case MoveCursorParam param:
                CommandHandler.Execute(param);
                break;

            case MouseClickParam param:
                CommandHandler.Execute(param);
                break;

            default:
                throw new Exception("Unknown command!");
        }
    }
}