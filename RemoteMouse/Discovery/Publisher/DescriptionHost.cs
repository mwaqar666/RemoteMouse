using System;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using RemoteMouse.DI.Contracts;
using Rssdp;

namespace RemoteMouse.Discovery.Publisher;

public class DescriptionHost(IScopedResourceFactory resourceFactory)
{
    // public IObservable<Unit> ServeDeviceDocument(SsdpDevice device)
    // {
    //     return Observable.Using(
    //         resourceFactory.GetResource<HttpListener>,
    //         resourceContainer => Observable.Create<Unit>(observer =>
    //         {
    //             var listener = resourceContainer.Resources;
    //
    //             // Add the prefix to the listener
    //             listener.Prefixes.Add(
    //                 $"{device.PresentationUrl.Scheme}://{device.PresentationUrl.Host}/"
    //             );
    //
    //             var deviceDocument = CreateDeviceDocument(device);
    //
    //             try
    //             {
    //                 listener.Start();
    //
    //                 observer.OnNext(Unit.Default);
    //
    //                 var subscription = Observable
    //                     .FromAsync(listener.GetContextAsync)
    //                     .Repeat()
    //                     .TakeWhile(_ => listener.IsListening)
    //                     .Subscribe(
    //                         context => HandleRequest(context, deviceDocument),
    //                         observer.OnError
    //                     );
    //
    //                 return () =>
    //                 {
    //                     subscription.Dispose();
    //
    //                     listener.Stop();
    //
    //                     listener.Close();
    //                 };
    //             }
    //             catch (Exception exception)
    //             {
    //                 observer.OnError(exception);
    //
    //                 return () => { };
    //             }
    //         })
    //     );
    // }

    public IObservable<Unit> ServeDeviceDocument(SsdpDevice device)
    {
        return Observable.Using(
            resourceFactory.GetResource<HttpListener>,
            resourceContainer => Observable.Create<Unit>(observer =>
            {
                var listener = resourceContainer.Resources;

                // Add the prefix to the listener
                listener.Prefixes.Add(
                    $"{device.PresentationUrl.Scheme}://{device.PresentationUrl.Host}/"
                );

                try
                {
                    listener.Start();

                    observer.OnNext(Unit.Default);

                    return ListenForRequests(listener, device);
                }
                catch (Exception exception)
                {
                    observer.OnError(exception);

                    return Disposable.Empty;
                }
            })
        );
    }

    private static IDisposable ListenForRequests(HttpListener listener, SsdpDevice device)
    {
        var deviceDocument = CreateDeviceDocument(device);

        return Observable
            .FromAsync(listener.GetContextAsync)
            .Repeat()
            .TakeWhile(_ => listener.IsListening)
            .Subscribe(
                context => HandleRequest(context, deviceDocument),
                () => CleanupListener(listener)
            );
    }

    private static void HandleRequest(HttpListenerContext context, string deviceDocument)
    {
        try
        {
            if (context.Request.Url is not { AbsolutePath: "/device-description.xml" })
            {
                SetResponse(context, HttpStatusCode.NotFound);

                return;
            }

            SetResponse(context, HttpStatusCode.OK, Encoding.UTF8.GetBytes(deviceDocument));
        }
        catch (Exception)
        {
            SetResponse(context, HttpStatusCode.InternalServerError);
        }
    }

    private static string CreateDeviceDocument(SsdpDevice device)
    {
        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "root",
                new XAttribute("xmlns", "urn:schemas-upnp-org:device-1-0"),
                new XElement("device",
                    new XElement("deviceType", $"urn:{device.DeviceTypeNamespace}:device:{device.DeviceType}:{device.DeviceVersion}"),
                    new XElement("friendlyName", device.FriendlyName),
                    new XElement("manufacturer", device.Manufacturer),
                    new XElement("modelName", device.ModelName),
                    new XElement("UDN", $"uuid:{device.Uuid}"),
                    new XElement("presentationURL", device.PresentationUrl)
                )
            )
        ).ToString();
    }

    private static void SetResponse(HttpListenerContext context, HttpStatusCode statusCode, byte[]? buffer = null)
    {
        try
        {
            context.Response.StatusCode = (int)statusCode;

            if (buffer is null) return;

            context.Response.ContentType = "application/xml";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        finally
        {
            context.Response.OutputStream.Close();
        }
    }

    private static void CleanupListener(HttpListener listener)
    {
        listener.Stop();
        listener.Close();
    }
}