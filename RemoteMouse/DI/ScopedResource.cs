using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DI.Contracts;

namespace RemoteMouse.DI;

public readonly struct ScopedResource<T>(IServiceScope scope, T resources) : IScopedResource<T>
{
    public T Resources => resources;

    public void Dispose()
    {
        scope.Dispose();
    }
}