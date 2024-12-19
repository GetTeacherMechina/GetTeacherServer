using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers;

[Route("")]
public class DefaultController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
