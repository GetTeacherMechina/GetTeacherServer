using System.Net.WebSockets;
using System.Threading.Tasks.Dataflow;
using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.Networking;

public record WebSocketReceiveResult(bool Success, string Message);

public interface IWebSocketSystem
{
	public void AddWebSocket(DbUser user, WebSocket webSocket, BufferBlock<WebSocketReceiveResult> messageQueue);

	public void RemoveWebSocket(DbUser user);

	public Task<WebSocketReceiveResult> ReceiveAsync(int clientId);

	public Task<bool> SendAsync<T>(int clientId, T message);
}