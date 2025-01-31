using System;
using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DI.Contracts;

namespace RemoteMouse.DI;

public class ScopedResourceFactory(IServiceProvider serviceProvider) : IScopedResourceFactory
{
    public IScopedResource<T> GetResource<T>() where T : notnull
    {
        var scope = serviceProvider.CreateScope();

        var t = scope.ServiceProvider.GetRequiredService<T>();

        return new ScopedResource<T>(scope, t);
    }

    public IScopedResource<(T1, T2)> GetResources<T1, T2>() where T1 : notnull where T2 : notnull
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();

        return new ScopedResource<(T1, T2)>(scope, (t1, t2));
    }

    public IScopedResource<(T1, T2, T3)> GetResources<T1, T2, T3>() where T1 : notnull where T2 : notnull where T3 : notnull
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();
        var t3 = scope.ServiceProvider.GetRequiredService<T3>();

        return new ScopedResource<(T1, T2, T3)>(scope, (t1, t2, t3));
    }

    public IScopedResource<(T1, T2, T3, T4)> GetResources<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();
        var t3 = scope.ServiceProvider.GetRequiredService<T3>();
        var t4 = scope.ServiceProvider.GetRequiredService<T4>();

        return new ScopedResource<(T1, T2, T3, T4)>(scope, (t1, t2, t3, t4));
    }

    public IScopedResource<(T1, T2, T3, T4, T5)> GetResources<T1, T2, T3, T4, T5>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
    {
        var scope = serviceProvider.CreateScope();

        var t1 = scope.ServiceProvider.GetRequiredService<T1>();
        var t2 = scope.ServiceProvider.GetRequiredService<T2>();
        var t3 = scope.ServiceProvider.GetRequiredService<T3>();
        var t4 = scope.ServiceProvider.GetRequiredService<T4>();
        var t5 = scope.ServiceProvider.GetRequiredService<T5>();

        return new ScopedResource<(T1, T2, T3, T4, T5)>(scope, (t1, t2, t3, t4, t5));
    }
}