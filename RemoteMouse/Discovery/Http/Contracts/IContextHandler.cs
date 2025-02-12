using System;
using System.Net;
using Rssdp;

namespace RemoteMouse.Discovery.Http.Contracts;

public interface IContextHandler
{
    public IContextHandler WithContext(HttpListenerContext context);

    public IObservable<string> HandleContext(SsdpRootDevice device);
}