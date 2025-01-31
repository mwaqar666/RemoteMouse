using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using Rssdp;

namespace RemoteMouse.Discovery.Publisher;

public class DescriptionHost(HttpListener listener)
{
    public IObservable<Unit> ServeDeviceDocument(SsdpDevice device)
    {
        return Observable.Create<Unit>(observer =>
        {
            // Must not create another listener if one is already listening
            if (listener.IsListening)
            {
                observer.OnError(new InvalidOperationException("Listener is already listening"));

                return () => { };
            }

            // Add the prefix to the listener
            var listenerPrefix = $"{device.PresentationUrl.Scheme}://{device.PresentationUrl.Host}/";
            if (!listener.Prefixes.Contains(listenerPrefix)) listener.Prefixes.Add(listenerPrefix);

            try
            {
                listener.Start();
                observer.OnNext(Unit.Default);

                var subscription = Observable
                    .FromAsync(listener.GetContextAsync)
                    .Repeat()
                    .TakeWhile(_ => listener.IsListening)
                    .Subscribe(
                        context => HandleRequest(context, device),
                        observer.OnError,
                        observer.OnCompleted
                    );

                return () =>
                {
                    subscription.Dispose();

                    listener.Stop();

                    listener.Close();
                };
            }
            catch (Exception exception)
            {
                observer.OnError(exception);

                return () => { };
            }
        });
    }

    private static void HandleRequest(HttpListenerContext context, SsdpDevice device)
    {
        try
        {
            if (context.Request.Url is not { AbsolutePath: "/device-description.xml" })
            {
                SetResponse(context, HttpStatusCode.NotFound);

                return;
            }

            SetResponse(context, HttpStatusCode.OK, Encoding.UTF8.GetBytes(CreateDeviceDocument(device)));
        }
        catch (Exception)
        {
            SetResponse(context, HttpStatusCode.InternalServerError);
        }
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
}