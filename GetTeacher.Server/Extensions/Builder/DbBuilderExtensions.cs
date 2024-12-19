using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Extensions.Builder;

public static class DbBuilderExtensions
{
	public static void UseGetTeacherDb(this WebApplicationBuilder builder)
	{
		string? connectionString = builder.Configuration.GetConnectionString("Default");
		if (connectionString == null)
		{
			// TODO: Logging
			Console.WriteLine("Connection string was null, please provide one in appsettings.json");
			return;
		}

		// Add identity services
		builder.Services
			.AddIdentityCore<DbUser>()
			.AddEntityFrameworkStores<GetTeacherDbContext>();

		// Add DbContext based on environment
		// Postgre for production and sqlite for development
		if (builder.Environment.IsDevelopment())
			builder.Services.AddSqlite(connectionString);
		else
			builder.Services.AddPostgreSql(connectionString);
	}

	private static void AddPostgreSql(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<GetTeacherDbContext>(options => options.UseNpgsql(connectionString));
	}

	private static void AddSqlite(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<GetTeacherDbContext>(options => options.UseSqlite(connectionString));
	}
}