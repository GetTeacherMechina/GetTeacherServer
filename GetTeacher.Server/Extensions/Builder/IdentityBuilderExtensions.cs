using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace GetTeacher.Server.Extensions.Builder;

public static class IdentityBuilderExtensions
{
	public static void AddGetTeacherIdentity(this WebApplicationBuilder builder)
	{
		// Get the configuration
		IConfiguration configuration = builder.Services
			.BuildServiceProvider()
			.CreateScope().ServiceProvider
			.GetRequiredService<IConfiguration>();

		string? allowedUserNameCharacters = configuration["IdentitySettings:AllowedUsernameCharacters"];
		if (allowedUserNameCharacters is null)
		{
			// TODO: Logging
			Console.WriteLine("IdentitySettings:AllowedUsernameCharacters was null, please provide one in appsettings.json");
			return;
		}

		builder.Services
			.AddIdentityCore<DbUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
				options.User.AllowedUserNameCharacters = allowedUserNameCharacters;
				options.User.RequireUniqueEmail = true;
			}).AddEntityFrameworkStores<GetTeacherDbContext>()
			.AddDefaultTokenProviders();
	}
}