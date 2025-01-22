using GetTeacher.Server.Extensions.App;
using GetTeacher.Server.Extensions.Builder;

namespace GetTeacher.Server;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
		builder.Services.AddControllers();
		builder.AddCorsPolicy();

		// Custom services via extension methods
		builder.AddGetTeacherServices();
		builder.AddGetTeacherIdentity();
		builder.AddJwtAuthentication();

		var app = builder.Build();

		app.UseCors();

		//// Redirects http requests to https
		//app.UseHttpsRedirection();

		// Authentication pipeline stage happens BEFORE the authorization
		app.UseAuthentication();
		app.UseAuthorization();

		// Add the WebSocket pipeline stage AFTER authentication
		app.UseGetTeacherWebSockets();

		app.MapControllers();

		// Run duh
		app.Run();
	}
}