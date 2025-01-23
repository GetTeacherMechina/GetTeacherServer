using GetTeacher.Server.Models.Authentication;
using GetTeacher.Server.Models.Authentication.Login;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Authentication;

[ApiController]
[Route("/api/v1/auth/[controller]")]
public class GoogleController(IJwtGenerator jwtGenerator, ILogger<GoogleController> logger, UserManager<DbUser> userManager, IConfiguration configuration) : ControllerBase
{
	private readonly IJwtGenerator jwtGenerator = jwtGenerator;
	private readonly ILogger<GoogleController> logger = logger;
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly IConfiguration configuration = configuration;

	[HttpPost]
	public async Task<IActionResult> ValidateGoogleToken([FromBody] GoogleTokenRequestModel request)
	{
		string? clientId = configuration["GoogleSettings:ClientId"];
		if (clientId is null)
		{
			logger.LogCritical("GoogleSettings are not set up correctly in the configuration");
			return BadRequest();
		}

		if (string.IsNullOrEmpty(request.IdToken))
			return BadRequest(new { error = "ID token is required." });

		try
		{
			var validPayload = await ValidateIdToken(request.IdToken);

			if (validPayload == null)
				return Unauthorized(new { error = "Invalid ID token." });

			DbUser? user = await userManager.FindByEmailAsync(validPayload.Email);
			if (user is null)
				return BadRequest();
			else if (!await userManager.IsEmailConfirmedAsync(user))
				return BadRequest();

			string? jwt = jwtGenerator.GenerateUserToken(user);
			if (jwt is null)
				return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred when generating a JWT.");

			return Ok(new LoginResponseModel { Result = "Login successful", JwtToken = jwt });
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
		}
	}

	private async Task<GoogleJsonWebSignature.Payload?> ValidateIdToken(string idToken)
	{
		string? clientId = configuration["GoogleSettings:ClientId"];
		if (clientId is null)
		{
			logger.LogCritical("GoogleSettings are not set up correctly in the configuration");
			return null;
		}

		var settings = new GoogleJsonWebSignature.ValidationSettings()
		{
			Audience = [clientId]
		};

		return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
	}
}