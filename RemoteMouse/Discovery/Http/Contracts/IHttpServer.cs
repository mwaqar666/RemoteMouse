using System;
using System.Net;
using Rssdp;

namespace RemoteMouse.Discovery.Http.Contracts;

public interface IHttpServer
{
    public IObservable<HttpListenerContext> StartListening(SsdpRootDevice device);

    public IContextHandler CreateContextHandler(HttpListenerContext httpListenerContext);
}