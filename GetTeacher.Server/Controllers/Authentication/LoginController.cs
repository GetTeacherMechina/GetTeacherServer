﻿using GetTeacher.Server.Models.Authentication.Login;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class LoginController(ITwoFactorAuthenticationManager twoFactorAuthenticationManager, IWebSocketSystem webSocketSystem, UserManager<DbUser> userManager, IUserStateTracker userStateTracker, IJwtGenerator jwtGenerator) : ControllerBase
{
	private readonly ITwoFactorAuthenticationManager twoFactorAuthenticationManager = twoFactorAuthenticationManager;
	private readonly IWebSocketSystem webSocketSystem = webSocketSystem;
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly IJwtGenerator jwtTokenGenerator = jwtGenerator;

	[HttpPost]
	public async Task<IActionResult> Login([FromBody] LoginRequestModel loginModel)
	{
		DbUser? user = await userManager.FindByEmailAsync(loginModel.Email);
		if (user == null)
			return Unauthorized(new LoginResponseModel { Result = "Invalid email or password" });
		var passwordResult = await userManager.CheckPasswordAsync(user, loginModel.Password);

		if (passwordResult)
		{
			string? jwt = jwtTokenGenerator.GenerateUserToken(user);
			if (jwt is null)
				return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred when generating a JWT.");

			// TODO: Check if in a call BEFORE drop-kicking if online
			// Drop-kick the previous client instance
			if (await userStateTracker.IsUserOnline(user))
				webSocketSystem.RemoveWebSocket(user);

			// TODO: MAKE MANDATORY
			// if (!user.EmailConfirmed)
			// {
			// 	await twoFactorAuthenticationManager.CreateAndSend2FaCode(user);
			// 
			// 	// Return 2FA required response
			// 	return Ok(new { RequiresTwoFactor = true, Message = "2FA code sent to your email." });
			// }

			return Ok(new LoginResponseModel { Result = "Login successful", JwtToken = jwt });
		}

		return Unauthorized(new LoginResponseModel { Result = "Invalid email or password" });
	}
}