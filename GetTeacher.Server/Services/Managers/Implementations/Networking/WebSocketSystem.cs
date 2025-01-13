using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;

namespace GetTeacher.Server.Services.Managers.Implementations.Networking;

public record WebSocketProfile(DbUser User, WebSocket Socket, BufferBlock<Interfaces.Networking.WebSocketReceiveResult> MessageQueue);

public class WebSocketSystem(ILogger<IWebSocketSystem> logger, IUserStateTracker userStateChecker) : IWebSocketSystem
{
	private const int maxMessageLength = 4096;

	private static readonly ConcurrentDictionary<int, WebSocketProfile> clients = new();

	private readonly ILogger<IWebSocketSystem> logger = logger;
	private readonly IUserStateTracker userStateChecker = userStateChecker;

	public void AddWebSocket(DbUser user, WebSocket webSocket, BufferBlock<Interfaces.Networking.WebSocketReceiveResult> messageQueue)
	{
		WebSocketProfile ws = new WebSocketProfile(user, webSocket, messageQueue);

		clients.AddOrUpdate(ws.User.Id, ws, (key, old) => ws);
		userStateChecker.SetOnline(new DbUser { Id = user.Id });
		logger.LogInformation("Client [{clientId}] WebSocket connected.", user.Id);
	}

	public void RemoveWebSocket(DbUser user)
	{
		userStateChecker.SetOffline(user);
		clients.TryRemove(user.Id, out _);
		logger.LogInformation("Client [{clientId}] WebSocket disconnected.", user.Id);
	}

	public async Task<Interfaces.Networking.WebSocketReceiveResult> ReceiveAsync(int clientId)
	{
		if (!clients.TryGetValue(clientId, out var ws))
			return new Interfaces.Networking.WebSocketReceiveResult(false, "");

		return await ws.MessageQueue.ReceiveAsync();
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