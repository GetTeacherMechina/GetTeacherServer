using GetTeacherServer.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacherServer.Controllers.Authentication;

[ApiController]
[Route("auth/[controller]")]
public class RegisterController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(RegisterModel registerModel)
    {
        return Ok();
    }
}