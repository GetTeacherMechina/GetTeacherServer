using GetTeacher.Server.Extensions.App;
using GetTeacher.Server.Extensions.Builder;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace GetTeacher.Server;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Services.AddControllers();

		// Adds the custom services to the service collection
		builder.UseGetTeacherServices();

		builder.AddJwtAuthentication();

		// Adds the identity services
		builder.Services.AddIdentityCore<DbUser>()
			.AddEntityFrameworkStores<GetTeacherDbContext>()
			.AddDefaultTokenProviders();

		builder.AddCorsPolicy();

		var app = builder.Build();

		app.UseCors();

		// Redirects http requests to https
		app.UseHttpsRedirection();

		// Authentication pipeline stage happens before the authorization
		app.UseAuthentication();
		app.UseAuthorization();

		// Add the WebSocket pipeline stage AFTER authentication
		app.UseGetTeacherWebSockets();

		app.MapControllers();

		// Run duh
		app.Run();
	}
}