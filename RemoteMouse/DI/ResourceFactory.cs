using System;
using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DI.Contracts;

namespace RemoteMouse.DI;

public class ResourceFactory(IServiceProvider serviceProvider) : IResourceFactory
{
    public IResourceContainer<T> GetResource<T>() where T : IDisposable
    {
        var scope = serviceProvider.CreateScope();

        var t = scope.ServiceProvider.GetRequiredService<T>();

        return new ResourceContainer<T>(scope, t);
    }

    public IResourceContainer<(T1, T2)> GetResources<T1, T2>() where T1 : IDisposable where T2 : IDisposable
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();

        return new ResourceContainer<(T1, T2)>(scope, (t1, t2));
    }

    public IResourceContainer<(T1, T2, T3)> GetResources<T1, T2, T3>() where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();
        var t3 = scope.ServiceProvider.GetRequiredService<T3>();

        return new ResourceContainer<(T1, T2, T3)>(scope, (t1, t2, t3));
    }

    public IResourceContainer<(T1, T2, T3, T4)> GetResources<T1, T2, T3, T4>() where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable where T4 : IDisposable
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();
        var t3 = scope.ServiceProvider.GetRequiredService<T3>();
        var t4 = scope.ServiceProvider.GetRequiredService<T4>();

        return new ResourceContainer<(T1, T2, T3, T4)>(scope, (t1, t2, t3, t4));
    }

    public IResourceContainer<(T1, T2, T3, T4, T5)> GetResources<T1, T2, T3, T4, T5>() where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable where T4 : IDisposable where T5 : IDisposable
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();
        var t3 = scope.ServiceProvider.GetRequiredService<T3>();
        var t4 = scope.ServiceProvider.GetRequiredService<T4>();
        var t5 = scope.ServiceProvider.GetRequiredService<T5>();

        return new ResourceContainer<(T1, T2, T3, T4, T5)>(scope, (t1, t2, t3, t4, t5));
    }
}