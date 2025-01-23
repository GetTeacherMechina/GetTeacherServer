using GetTeacher.Server.Models.Profile;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Profile;

[Controller]
[Route("/api/v1/reset-password/code-check")]
public class CodeCheckController(ITokenStore resetPasswordCodeManager) : ControllerBase
{
	private readonly ITokenStore resetPasswordCodeManager = resetPasswordCodeManager;

	[HttpPost]
	public IActionResult CodeCheck([FromBody] CodeCheckModel codeCheckModel)
	{
		if (!codeCheckModel.Code.Equals(resetPasswordCodeManager.GetToken(codeCheckModel.Token)))
			return BadRequest("Wrong password reset token.");

		return Ok(new { });
	}
}
