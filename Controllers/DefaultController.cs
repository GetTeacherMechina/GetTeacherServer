using Microsoft.AspNetCore.Mvc;

namespace GetTeacherServer.Controllers;

[Route("")]
public class DefaultController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Redirect("auth/login");
    }
}
