using System.Net.WebSockets;
using System.Runtime.Serialization;

namespace GetTeacher.Server.Services.Managers.Interfaces.Networking;

public interface IWebSocketHandler
{
	public void AddWebSocket(int clientId, WebSocket webSocket);

	public void SetMessageReceivedHandler(int clientId, Func<string, Task<object?>> messageReceived);

	public Task SendAsync<T>(int clientId, T message) where T : ISerializable;
}