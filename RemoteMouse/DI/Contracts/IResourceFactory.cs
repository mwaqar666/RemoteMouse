using System;

namespace RemoteMouse.DI.Contracts;

public interface IResourceFactory
{
    public IResourceContainer<T1> GetResource<T1>() where T1 : IDisposable;

    public IResourceContainer<(T1, T2)> GetResources<T1, T2>() where T1 : IDisposable where T2 : IDisposable;

    public IResourceContainer<(T1, T2, T3)> GetResources<T1, T2, T3>() where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable;

    public IResourceContainer<(T1, T2, T3, T4)> GetResources<T1, T2, T3, T4>() where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable where T4 : IDisposable;

    public IResourceContainer<(T1, T2, T3, T4, T5)> GetResources<T1, T2, T3, T4, T5>() where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable where T4 : IDisposable where T5 : IDisposable;
}