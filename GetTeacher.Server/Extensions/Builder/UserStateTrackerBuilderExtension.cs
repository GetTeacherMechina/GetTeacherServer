using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Managers.Implementations.UserStateTracker;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;
using StackExchange.Redis;

namespace GetTeacher.Server.Extensions.Builder;

public static class UserStateTrackerBuilderExtension
{
	public static void UseGetTeacherUserStateTracker(this WebApplicationBuilder builder)
	{
		if (builder.Environment.IsRelease())
			builder.Services.AddRedisUserStateTracker();
		else if (builder.Environment.IsDevelopment())
			builder.Services.AddLocalUserStateTracker();
		else
			Console.WriteLine("Error: Environment unrecognized");
	}

	private static void AddLocalUserStateTracker(this IServiceCollection services)
	{
		// TODO: Logger
		Console.WriteLine("Adding LocalUserStateChecker");
		services.AddScoped<IUserStateStatus, LocalUserStateStatus>();
	}

	private static void AddRedisUserStateTracker(this IServiceCollection services)
	{
		// TODO: Logger
		Console.WriteLine("Adding RedisUserStateChecker");

		string? connectionIp = Environment.GetEnvironmentVariable("REDIS_CONN_IP");
		if (connectionIp is null)
		{
			// TODO: Logging
			Console.WriteLine("REDIS_CONN_IP environment variable was not set, please provide one");
			return;
		}

		Console.WriteLine("Redis connection IP: {0}", connectionIp);
		services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionIp, true)));
		services.AddStackExchangeRedisCache(options => options.Configuration = connectionIp);
		services.AddScoped<IUserStateStatus, RedisUserStateStatus>();
		services.AddScoped<IRedisCache, RedisCache>();
	}
}