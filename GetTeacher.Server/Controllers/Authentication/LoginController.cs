using GetTeacher.Server.Models.Authentication.Login;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class LoginController(UserManager<DbUser> userManager, IJwtGenerator jwtTokenGenerator) : ControllerBase
{
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly IJwtGenerator jwtTokenGenerator = jwtTokenGenerator;

	[HttpPost]
	public async Task<IActionResult> Login([FromBody] LoginRequestModel loginModel)
	{
		DbUser? userResult = await userManager.FindByEmailAsync(loginModel.Email);
		if (userResult == null)
			return Unauthorized(new LoginResponseModel { Result = "Invalid email or password" });

		var passwordResult = await userManager.CheckPasswordAsync(userResult, loginModel.Password);

		if (passwordResult)
		{
			string? jwt = jwtTokenGenerator.GenerateUserToken(userResult);
			if (jwt is null)
				return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred when generating a JWT.");

			return Ok(new LoginResponseModel { Result = "Login successful", JwtToken = jwt });
		}

		return Unauthorized(new LoginResponseModel { Result = "Invalid email or password" });
	}
}