using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace GetTeacher.Server.Services.Managers.Implementations.Networking;

public class WebSocketProfile(int clientId, WebSocket ws, Func<string, Task<object?>> messageHandler)
{
	public int clientId = clientId;
	public WebSocket webSocket = ws;
	public Func<string, Task<object?>> messageReceived = messageHandler;
}

public class WebSocketManager : IWebSockerManager
{
	private readonly ConcurrentDictionary<int, WebSocketProfile> _clients = new();

	public void AddWebSocket(int clientId, WebSocket webSocket)
	{
		WebSocketProfile ws = new WebSocketProfile(clientId, webSocket, async (str) => null);
		_ = WebSocketHandler(ws);
	}

    public void SetMessageReceivedHandler(int clientId, Func<string, Task<object?>> messageReceived)
    {
        if (!_clients.TryGetValue(clientId, out var ws))
            return;

        ws.messageReceived = messageReceived;
    }

    public async Task SendAsync<T>(int clientId, T message) where T : ISerializable
	{
		// "out var ws" - wow - amazing syntax. breathtaking.
		if (!_clients.TryGetValue(clientId, out var ws))
			return;

		if (ws.webSocket.State != WebSocketState.Open) // well ill be
			return;

		var json = JsonSerializer.Serialize(message);
		var bytes = Encoding.UTF8.GetBytes(json);
		await ws.webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
	}

	private async Task OnClientConnectedAsync(WebSocketProfile ws)
	{
		_clients.AddOrUpdate(ws.clientId, ws, (key, old) => ws);
	}

	private async Task OnClientDisconnectedAsync(WebSocketProfile ws)
	{
		if (ws.webSocket.State != WebSocketState.Closed && ws.webSocket.State != WebSocketState.Aborted)
			await ws.webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Unexpected error", CancellationToken.None);

		_clients.TryRemove(ws.clientId, out _);
	}

	private async Task WebSocketHandler(WebSocketProfile ws)
	{
		await OnClientConnectedAsync(ws);

		var buffer = new byte[4096]; // 4096 gorgeous number
		try
		{
			// TODO: what is cancellation token fucking used for (guess: cancell an action that was already made?)
			while (ws.webSocket.State == WebSocketState.Open)
			{
				var result = await ws.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (result.MessageType == WebSocketMessageType.Close)
					throw new Exception();

				if (result.MessageType == WebSocketMessageType.Text)
				{
					var jsonString = Encoding.UTF8.GetString(buffer, 0, result.Count);
					await ws.messageReceived(jsonString);
				}
			}
		}
		catch // even handling unexpected errors (aside from abortions and closed connections).
		{
			await OnClientDisconnectedAsync(ws);
		}
	}
}