using System;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Rssdp;

namespace RemoteMouse.Discovery.Http.Context;

public class HttpContextHandler : ContextHandler
{
    public override IObservable<string> HandleContext(SsdpRootDevice device)
    {
        return Observable.Create<string>(observer =>
        {
            if (Context.Request.Url is not { AbsolutePath: "/device-description.xml" })
            {
                observer.OnNext($"Invalid Http request: [{Context.Request.Url}]");

                return SetResponse(HttpStatusCode.NotFound);
            }

            observer.OnNext($"Incoming Http request: [{Context.Request.Url}]");

            return SetResponse(HttpStatusCode.OK, Encoding.UTF8.GetBytes(device.ToDescriptionDocument()));
        });
    }

    private IDisposable SetResponse(HttpStatusCode statusCode, byte[]? buffer = null)
    {
        try
        {
            Context.Response.StatusCode = (int)statusCode;

            if (buffer is null) return Disposable.Empty;

            Context.Response.ContentType = "application/xml";
            Context.Response.ContentLength64 = buffer.Length;
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        finally
        {
            Context.Response.OutputStream.Close();
        }

        return Disposable.Empty;
    }
}