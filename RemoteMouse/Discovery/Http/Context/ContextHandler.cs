using System;
using System.Net;
using RemoteMouse.Discovery.Http.Contracts;
using Rssdp;

namespace RemoteMouse.Discovery.Http.Context;

public abstract class ContextHandler : IContextHandler
{
    private HttpListenerContext? _context;

    protected HttpListenerContext Context
    {
        get => _context ?? throw new NullReferenceException("HttpListenerContext must be set using WithContext before calling the HandleContext");
        private set => _context = value;
    }

    public IContextHandler WithContext(HttpListenerContext context)
    {
        Context = context;

        return this;
    }

    public abstract IObservable<string> HandleContext(SsdpRootDevice device);
}