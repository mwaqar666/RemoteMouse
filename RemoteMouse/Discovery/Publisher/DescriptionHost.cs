using System;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using RemoteMouse.DI.Contracts;
using Rssdp;

namespace RemoteMouse.Discovery.Publisher;

public class DescriptionHost(IResourceFactory resourceFactory)
{
    public IObservable<string> ServeDeviceDocument(SsdpDevice device)
    {
        return Observable.Using(
            resourceFactory.GetResource<HttpListener>,
            resourceContainer => Observable.Create<string>(observer =>
            {
                var httpListener = resourceContainer.Resources;

                httpListener.Prefixes.Add(
                    $"{device.PresentationUrl.Scheme}://{device.PresentationUrl.Host}/"
                );

                try
                {
                    httpListener.Start();

                    observer.OnNext(device.PresentationUrl.Host);

                    var deviceDocument = CreateDeviceDocument(device);

                    var subscription = Observable
                        .FromAsync(httpListener.GetContextAsync)
                        .Repeat()
                        .TakeWhile(_ => httpListener.IsListening)
                        .Subscribe(
                            context => HandleRequest(context, deviceDocument),
                            observer.OnError,
                            observer.OnCompleted
                        );

                    return () =>
                    {
                        subscription.Dispose();

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
        XNamespace xNamespace = "urn:schemas-upnp-org:device-1-0";

        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(xNamespace + "root",
                new XElement(xNamespace + "device",
                    new XElement(xNamespace + "deviceType", $"urn:{device.DeviceTypeNamespace}:device:{device.DeviceType}:{device.DeviceVersion}"),
                    new XElement(xNamespace + "friendlyName", device.FriendlyName),
                    new XElement(xNamespace + "manufacturer", device.Manufacturer),
                    new XElement(xNamespace + "modelName", device.ModelName),
                    new XElement(xNamespace + "UDN", $"uuid:{device.Uuid}"),
                    new XElement(xNamespace + "presentationURL", device.PresentationUrl)
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
}