using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class JwtAuthenticator(IAuthenticationService authenticationService, IOptionsMonitor<JwtBearerOptions> optionsMonitor) : IJwtAuthenticator
{
	private readonly IAuthenticationService authenticationService = authenticationService;
	private readonly JwtBearerOptions jwtOptions = optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);

	public ClaimsPrincipal? ValidateToken(string token)
	{
		var tokenHandler = new JwtSecurityTokenHandler();

		try
		{
			var principal = tokenHandler.ValidateToken(token, jwtOptions.TokenValidationParameters, out var validatedToken);

			if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				return principal;
		}
		catch
		{
			// Handle invalid token
		}

		return null;
	}

	public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context)
	{
		return await authenticationService.AuthenticateAsync(context, JwtBearerDefaults.AuthenticationScheme);
	}
}
