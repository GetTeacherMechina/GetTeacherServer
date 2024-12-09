using GetTeacherServer.Models.Authentication.Register;
using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacherServer.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly UserManager<GetTeacherUserIdentity> userManager;

    public RegisterController(UserManager<GetTeacherUserIdentity> userManager)
    {
        this.userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequestModel registerModel)
    {
        var user = new GetTeacherUserIdentity
        {
            UserName = registerModel.Username,
            Email = registerModel.Email
        };

        var result = await userManager.CreateAsync(user, registerModel.Password);

        if (result.Succeeded)
        {
            return Ok(new { Message = "Registration successful" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return BadRequest(ModelState);
    }
}