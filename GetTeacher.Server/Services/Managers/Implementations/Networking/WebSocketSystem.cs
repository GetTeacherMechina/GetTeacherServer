using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;

namespace GetTeacher.Server.Services.Managers.Implementations.Networking;

public record WebSocketProfile(int ClientId, WebSocket Socket);

public class WebSocketSystem(ILogger<WebSocketSystem> logger, IUserStateChecker userStateChecker) : IWebSocketSystem
{
	private static readonly ConcurrentDictionary<int, WebSocketProfile> clients = new();

	private readonly ILogger<WebSocketSystem> logger = logger;
	private readonly IUserStateChecker userStateChecker = userStateChecker;

	public void AddWebSocket(int clientId, WebSocket webSocket)
	{
		WebSocketProfile ws = new WebSocketProfile(clientId, webSocket);

		clients.AddOrUpdate(ws.ClientId, ws, (key, old) => ws);
		userStateChecker.UpdateUserOnline(new DbUser { Id = ws.ClientId }, true);
	}

	public async void RemoveWebSocket(int clientId)
	{
		userStateChecker.UpdateUserOnline(new DbUser { Id = clientId }, false);
		if (!clients.TryRemove(clientId, out WebSocketProfile? ws))
			return;

		try
		{
			if (ws.Socket.State != WebSocketState.Closed && ws.Socket.State != WebSocketState.Aborted)
				await ws.Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, CancellationToken.None);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An unexpected error occurred while closing the WebSocket.");
		}
	}

	public async Task<bool> SendAsync<T>(int clientId, T message)
	{
		// "out var ws" - wow - amazing syntax. breathtaking.
		if (!clients.TryGetValue(clientId, out var ws))
			return false;

		if (ws.Socket.State != WebSocketState.Open) // well ill be
			return false;

		var json = JsonSerializer.Serialize(message);
		var bytes = Encoding.UTF8.GetBytes(json);

		try
		{
			await ws.Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An unexpected error occurred while writing to a WebSocket: [{clientId}].", clientId);
			return false;
		}

		return true;
	}
}