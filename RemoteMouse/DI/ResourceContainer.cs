using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DI.Contracts;

namespace RemoteMouse.DI;

public readonly struct ResourceContainer<T>(IServiceScope scope, T resources) : IResourceContainer<T>
{
    public T Resources => resources;

    public void Dispose()
    {
        scope.Dispose();
    }
}