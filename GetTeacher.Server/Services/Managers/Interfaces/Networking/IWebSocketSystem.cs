using System.Net.WebSockets;
using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.Networking;

public interface IWebSocketSystem
{
	public void AddWebSocket(DbUser user, WebSocket webSocket);

	public void RemoveWebSocket(DbUser user);

	public Task<bool> SendAsync<T>(int clientId, T message);
}