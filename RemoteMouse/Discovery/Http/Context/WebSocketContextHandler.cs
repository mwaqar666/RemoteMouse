using System;
using System.Net.WebSockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using RemoteMouse.DeviceControl;
using Rssdp;

namespace RemoteMouse.Discovery.Http.Context;

public class WebSocketContextHandler(CommandExecutor commandExecutor) : ContextHandler
{
    private byte[] _buffer = new byte[1024];
    private WebSocket? _webSocket;

    private WebSocket Websocket => _webSocket ?? throw new NullReferenceException("Websocket is not initialized");

    public override IObservable<string> HandleContext(SsdpRootDevice device)
    {
        return Observable.Create<string>(observer =>
        {
            _buffer = new byte[1024];

            Observable
                .FromAsync(() => Context.AcceptWebSocketAsync(null))
                .Do(context => observer.OnNext($"Incoming Websocket request: [{context.RequestUri}]"))
                .Select(SetIncomingClient)
                .SelectMany(ListenForMessages)
                .Repeat()
                .TakeWhile(WebSocketEmitsMessages)
                .Select(DeserializeBinaryData)
                .Do(result => observer.OnNext($"Incoming Websocket message: [{result}]"))
                .Do(commandExecutor.HandleCommand)
                .Subscribe();

            return Disposable.Empty;
        });
    }

    private WebSocket SetIncomingClient(HttpListenerWebSocketContext context)
    {
        return _webSocket = context.WebSocket;
    }

    private Task<WebSocketReceiveResult> ListenForMessages(WebSocket webSocket)
    {
        var arraySegment = new ArraySegment<byte>(_buffer);

        return webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
    }

    private bool WebSocketEmitsMessages(WebSocketReceiveResult result)
    {
        return Websocket.State is WebSocketState.Open && result.MessageType is WebSocketMessageType.Binary;
    }

    private dynamic DeserializeBinaryData(WebSocketReceiveResult result)
    {
        return MessagePackSerializer.Deserialize<dynamic>(
            new ReadOnlyMemory<byte>(_buffer, 0, result.Count),
            ContractlessStandardResolver.Options
        );
    }
}