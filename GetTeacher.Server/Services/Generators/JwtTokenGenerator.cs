using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GetTeacher.Server.Extensions.Collection;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace GetTeacher.Server.Services.Generators;

public class JwtTokenGenerator(IConfiguration configuration, UserManager<DbUser> userManager)
{
	private readonly IConfiguration configuration = configuration;
	private readonly UserManager<DbUser> userManager = userManager;

	public async Task<string> GenerateUserToken(DbUser user)
	{
		// Add basic email username and JwtId claims
		ICollection<Claim> claims =
		[
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

			// TODO: Track JTI for force expiration?
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		];

		claims.AddRange((await userManager.GetClaimsAsync(user)).AsQueryable());
		// TODO: Add ClaimTypes.Role for roles

		return GenerateToken(claims, TimeSpan.FromMinutes(60));
	}

	private string GenerateToken(IEnumerable<Claim> claims, TimeSpan tokenLifetime)
	{
		string? key = configuration["JwtSettings:Key"];
		string? issuer = configuration["JwtSettings:Issuer"];
		string? audience = configuration["JwtSettings:Audience"];
		if (key == null || issuer == null || audience == null)
		{
			// Something wrong in the configuration, JwtSettings is not set up correctly
			// TODO: Change to a [critical] using logger
			Console.WriteLine("JwtSettings is not set up correctly");
			return string.Empty;
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