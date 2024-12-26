using System.Net.WebSockets;

namespace GetTeacher.Server.Services.Managers.Interfaces.Networking;

public interface IWebSocketSystem
{
	public void AddWebSocket(int clientId, WebSocket webSocket);

	public void RemoveWebSocket(int clientId);

	public Task<bool> SendAsync<T>(int clientId, T message);
}