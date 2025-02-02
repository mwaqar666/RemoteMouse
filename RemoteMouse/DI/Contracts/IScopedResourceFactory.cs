namespace RemoteMouse.DI.Contracts;

public interface IScopedResourceFactory
{
    public IResourceContainer<T1> GetResource<T1>() where T1 : notnull;

    public IResourceContainer<(T1, T2)> GetResources<T1, T2>() where T1 : notnull where T2 : notnull;

    public IResourceContainer<(T1, T2, T3)> GetResources<T1, T2, T3>() where T1 : notnull where T2 : notnull where T3 : notnull;

    public IResourceContainer<(T1, T2, T3, T4)> GetResources<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull;

    public IResourceContainer<(T1, T2, T3, T4, T5)> GetResources<T1, T2, T3, T4, T5>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull;
}