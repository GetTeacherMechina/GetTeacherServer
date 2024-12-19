using System.Security.Claims;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;

namespace GetTeacher.Server.Extensions.App;

public static class WebSocketAppExtension
{
	public static void UseGetTeacherWebSockets(this WebApplication app, IWebSockerManager webSocketManager)
	{
		app.UseWebSockets(); // nice informative

		app.Map("/api/v1/websocket", async context => //connecting to the given path
		{
			var user = context.User;
			if (user is null || user.Identity is null || user.Identity.IsAuthenticated is false)
				return;

			Claim? idClaim = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
			if (idClaim is null || !int.TryParse(idClaim.Value, out int userId))
				return;

			if (context.WebSockets.IsWebSocketRequest) //accept WebSocket requests from user (create connection)
			{
				var ws = await context.WebSockets.AcceptWebSocketAsync();
				webSocketManager.AddWebSocket(userId, ws);
			}
			else
				//classic error handling
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
		});
	}
}