using GetTeacherServer.Models.Authentication.Login;
using GetTeacherServer.Services.Database.Models;
using GetTeacherServer.Services.Generators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacherServer.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class LoginController : ControllerBase
{
    private readonly UserManager<DbUser> userManager;
    private readonly JwtTokenGenerator jwtTokenGenerator;

    public LoginController(UserManager<DbUser> signInManager, JwtTokenGenerator jwtTokenGenerator)
    {
        this.userManager = signInManager;
        this.jwtTokenGenerator = jwtTokenGenerator;
    }

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