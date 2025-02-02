using System;

namespace RemoteMouse.DI.Contracts;

public interface IResourceContainer<out T> : IDisposable
{
    T Resources { get; }
}