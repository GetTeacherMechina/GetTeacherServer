using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
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

			// Add the websocket to the handler
			webSocketSystem.AddWebSocket(clientId.Value, ws);
			context.User = claimsPrincipal;
		});
	}
}