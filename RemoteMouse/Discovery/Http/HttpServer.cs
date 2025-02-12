using System;
using System.Net;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using RemoteMouse.DI.Contracts;
using RemoteMouse.Discovery.Http.Context;
using RemoteMouse.Discovery.Http.Contracts;
using Rssdp;

namespace RemoteMouse.Discovery.Http;

public class HttpServer(IResourceFactory resourceFactory, IServiceProvider serviceProvider) : IHttpServer
{
    public IObservable<HttpListenerContext> StartListening(SsdpRootDevice device)
    {
        return Observable.Using(
            resourceFactory.GetResource<HttpListener>,
            resourceContainer => Observable.Create<HttpListenerContext>(observer =>
            {
                var httpListener = resourceContainer.Resources;

                var devicePresentationBaseUrl = $"{device.PresentationUrl.Scheme}://{device.PresentationUrl.Host}/";

                httpListener.Prefixes.Add(devicePresentationBaseUrl);

                try
                {
                    httpListener.Start();

                    Console.WriteLine($"Listening on: {devicePresentationBaseUrl}");

                    Observable
                        .FromAsync(httpListener.GetContextAsync)
                        .Repeat()
                        .TakeWhile(_ => httpListener.IsListening)
                        .Subscribe(observer);

                    return () =>
                    {
                        Console.WriteLine("Shutting down Http listener");

                        httpListener.Stop();

                        httpListener.Close();
                    };
                }
                catch (Exception exception)
                {
                    observer.OnError(exception);

                    return httpListener.Close;
                }
            })
        );
    }

    public IContextHandler CreateContextHandler(HttpListenerContext httpListenerContext)
    {
        return ResolveContextHandlerFromContainer(httpListenerContext).WithContext(httpListenerContext);
    }

    private IContextHandler ResolveContextHandlerFromContainer(HttpListenerContext httpListenerContext)
    {
        if (httpListenerContext.Request.IsWebSocketRequest) return serviceProvider.GetRequiredService<WebSocketContextHandler>();

        return serviceProvider.GetRequiredService<HttpContextHandler>();
    }
}