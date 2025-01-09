using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks.Dataflow;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;

namespace GetTeacher.Server.Extensions.App;

public static class WebSocketAppExtension
{
	public static void UseGetTeacherWebSockets(this WebApplication app)
	{
		app.UseWebSockets();

		// TODO: Add error handling
		// Maps the websocket endpoint to the logic down below
		app.Map("/api/v1/websocket", async context =>
		{
			if (!context.WebSockets.IsWebSocketRequest)
				return;

			var ws = await context.WebSockets.AcceptWebSocketAsync();

			// TODO: Eh I think it's fine for a JWT, but maybe refine?
			// Read the first message from the socket
			var buffer = new byte[4096];
			var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			if (result.MessageType != WebSocketMessageType.Text)
				return;

			// Get the required scoped services
			IServiceScope serviceScope = app.Services.CreateScope();
			ILogger<IWebSocketSystem> logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<IWebSocketSystem>>();
			IJwtAuthenticator jwtAuthenticator = serviceScope.ServiceProvider.GetRequiredService<IJwtAuthenticator>();
			IPrincipalClaimsQuerier principalClaimsQuerier = serviceScope.ServiceProvider.GetRequiredService<IPrincipalClaimsQuerier>();
			IWebSocketSystem webSocketSystem = serviceScope.ServiceProvider.GetRequiredService<IWebSocketSystem>();

			// The first message from a WebSocket should always be the JWT associated with the client
			var jwt = Encoding.UTF8.GetString(buffer, 0, result.Count);
			ClaimsPrincipal? claimsPrincipal = jwtAuthenticator.ValidateToken(jwt);
			if (claimsPrincipal is null)
				return;

			// Get the ID from the JWT claim
			int? clientId = principalClaimsQuerier.GetId(claimsPrincipal);
			if (clientId is null)
				return;

			// Add the WebSocket to the system
			context.User = claimsPrincipal;
			BufferBlock<Services.Managers.Interfaces.Networking.WebSocketReceiveResult> messages = new BufferBlock<Services.Managers.Interfaces.Networking.WebSocketReceiveResult>();
			webSocketSystem.AddWebSocket(new DbUser { Id = clientId.Value }, ws, messages);

			// TODO: I think there's no way to better implement this, but better safe than sorry: check again
			//  Keep the WebSocket alive by not exiting the context in which it was created
			while (ws.State == WebSocketState.Open)
			{
				byte[] messageBuffer = new byte[4096];
				try
				{
					System.Net.WebSockets.WebSocketReceiveResult messageResult = await ws.ReceiveAsync(messageBuffer, CancellationToken.None);

					ArraySegment<byte> message = new ArraySegment<byte>(messageBuffer, 0, messageResult.Count);
					string messageStr = Encoding.UTF8.GetString(message);
					messages.Post(new Services.Managers.Interfaces.Networking.WebSocketReceiveResult(true, messageStr));
				}
				catch
				{
					messages.Post(new Services.Managers.Interfaces.Networking.WebSocketReceiveResult(false, ""));
					break;
				}
			}
			// Imagine nesting "try-catch" blocks, couldn't be me
			try
			{
				await ws.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, CancellationToken.None);
				ws.Dispose();
			}
			catch
			{

			}

			webSocketSystem.RemoveWebSocket(new DbUser { Id = clientId.Value });

			// Due to the WebSocket being created from the "context" exiting this call frame causes the WebSocket to die
		});
	}
}