using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;

namespace GetTeacher.Server.Services.Managers.Implementations.Networking;

public class WebSocketProfile(int ClientId, WebSocket Socket)
{
    public int ClientId { get; set; } = ClientId;
    public WebSocket Socket { get; set; } = Socket;
    public Func<string, Task<object?>> MessageHandler { get; set; } = str => Task.FromResult<object?>(null);

}

public class WebSocketManager : IWebSockerManager
{
    private readonly ConcurrentDictionary<int, WebSocketProfile> _clients = new();

    public void AddWebSocket(int clientId, WebSocket webSocket)
        => WebSocketHandler(new WebSocketProfile(clientId, webSocket));

    public void SetMessageReceivedHandler(int clientId, Func<string, Task<object?>> messageReceived)
    {
        if (!_clients.TryGetValue(clientId, out var ws))
            return;

        ws.MessageHandler = messageReceived;
    }

    public async Task SendAsync<T>(int clientId, T message) where T : ISerializable
    {
        // "out var ws" - wow - amazing syntax. breathtaking.
        if (!_clients.TryGetValue(clientId, out var ws))
            return;

        if (ws.Socket.State != WebSocketState.Open) // well ill be
            return;

        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await ws.Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async void WebSocketHandler(WebSocketProfile ws)
    {
        await OnClientConnectedAsync(ws);

        var buffer = new byte[4096]; // 4096 gorgeous number
        try
        {
            // TODO: what is cancellation token fucking used for (guess: cancell an action that was already made?)
            while (ws.Socket.State == WebSocketState.Open)
            {
                var result = await ws.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    throw new Exception();

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var jsonString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await ws.MessageHandler(jsonString);
                }
            }
        }
        catch // Catch errors to prevent crashes
        {

        }
        finally
        {
            // Nevermind if a client disconnected due to an exception or a controlled stop, we handle the disconnection
            await OnClientDisconnectedAsync(ws);
        }
    }

    private async Task OnClientConnectedAsync(WebSocketProfile ws)
    {
        _clients.AddOrUpdate(ws.ClientId, ws, (key, old) => ws);

        // Suppress warning: we may need this function to be async sometime
        await Task.CompletedTask;
    }

    private async Task OnClientDisconnectedAsync(WebSocketProfile ws)
    {
        if (ws.Socket.State != WebSocketState.Closed && ws.Socket.State != WebSocketState.Aborted)
            await ws.Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, CancellationToken.None);

        _clients.TryRemove(ws.ClientId, out _);
    }
}