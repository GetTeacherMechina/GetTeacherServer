using GetTeacher.Server.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Extensions.Builder;

public static class DbBuilderExtensions
{
	public static void UseGetTeacherDb(this WebApplicationBuilder builder)
	{
		string? connectionString = builder.Configuration.GetConnectionString("Default");
		if (connectionString is null)
		{
			// TODO: Logging
			Console.WriteLine("ConnectionStrings:Default string was null, please provide one in appsettings.json");
			return;
		}

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