using GetTeacher.Server.Models.Authentication.Login;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Generators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class LoginController(UserManager<DbUser> userManager, JwtTokenGenerator jwtTokenGenerator) : ControllerBase
{
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly JwtTokenGenerator jwtTokenGenerator = jwtTokenGenerator;

	[HttpPost]
	public async Task<IActionResult> Login([FromBody] LoginRequestModel loginModel)
	{
		DbUser? userResult = await userManager.FindByEmailAsync(loginModel.Email);
		if (userResult == null)
			return Unauthorized(new LoginResponseModel { Result = "Invalid email or password" });

		var passwordResult = await userManager.CheckPasswordAsync(userResult, loginModel.Password);

		if (passwordResult)
		{
			string jwtToken = await jwtTokenGenerator.GenerateUserToken(userResult);
			return Ok(new LoginResponseModel { Result = "Login successful", JwtToken = jwtToken });
		}

		return Unauthorized(new LoginResponseModel { Result = "Invalid email or password" });
	}
}