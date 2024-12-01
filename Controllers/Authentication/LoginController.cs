using GetTeacherServer.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacherServer.Controllers.Authentication;

[ApiController]
[Route("auth/[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(LoginModel loginModel)
    {
        return Ok();
    }
}