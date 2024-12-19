using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GetTeacher.Server.Extensions.Builder;

public static class AuthenticationBuilderExtensions
{
	public static void AddCorsPolicy(this WebApplicationBuilder builder)
	{
		// Add CORS policy
		builder.Services.AddCors(options =>
		{
			//TODO: actually manage CORS
			options.AddDefaultPolicy(policy =>
			{
				policy.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			});
		});
	}

	public static void AddJwtAuthentication(this WebApplicationBuilder builder)
	{
		// Add JWT authentication scheme
		builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				// JWT options
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
					ValidAudience = builder.Configuration["JwtSettings:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
				};

				// Custom JWT event handlers
				options.Events = new JwtBearerEvents
				{
					OnTokenValidated = async context =>
					{
						// Yonatan TODO: Add custom database driven claims here maybe?
						await Task.CompletedTask;
					},
					OnAuthenticationFailed = context =>
					{
						// Log or handle authentication failures
						return Task.CompletedTask;
					},
					OnMessageReceived = context =>
					{
						if (context.Request.Query.ContainsKey("access_token"))
						{
							context.Token = context.Request.Query["access_token"];
						}

						return Task.CompletedTask;
					},
					OnChallenge = context =>
					{
						// Authentication fails
						// context.Response.Headers.Add("some-header", "Authentication failed");
						return Task.CompletedTask;
					}
				};
			});
	}
}