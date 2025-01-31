using System;

namespace RemoteMouse.DI.Contracts;

public interface IScopedResource<out T> : IDisposable
{
    T Resources { get; }
}