using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace GetTeacher.Server.Services.Generators;

public class JwtGenerator(ILogger<JwtGenerator> logger, IConfiguration configuration)
{
	private readonly ILogger<JwtGenerator> logger = logger;
	private readonly IConfiguration configuration = configuration;

	public string? GenerateUserToken(DbUser user)
	{
		// Add userId and JwtId
		ICollection<Claim> claims =
		[
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Email, user.Email!.ToString()),

			// TODO: Track JTI for force expiration?
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		];

		return GenerateToken(claims, TimeSpan.FromMinutes(60));
	}

	private string? GenerateToken(IEnumerable<Claim> claims, TimeSpan tokenLifetime)
	{
		string? key = configuration["JwtSettings:Key"];
		string? issuer = configuration["JwtSettings:Issuer"];
		string? audience = configuration["JwtSettings:Audience"];
		if (key == null || issuer == null || audience == null)
		{
			// Something wrong in the configuration, JwtSettings is not set up correctly
			// TODO: Change to a [critical] using logger
			logger.LogCritical("JwtSettings is not set up correctly");
			return null;
		}

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.Add(tokenLifetime),
			signingCredentials: signingCredentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}