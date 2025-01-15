using GetTeacher.Server.Models.Profile;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Profile;

[Controller]
[Route("/api/v1/reset-password/code-check")]
public class CodeCheckController(IResetPasswordTokenStore resetPasswordCodeManager) : ControllerBase
{
	private readonly IResetPasswordTokenStore resetPasswordCodeManager = resetPasswordCodeManager;

	[HttpPost]
	public IActionResult CodeCheck([FromBody] CodeCheckModel codeCheckModel)
	{
		if (!codeCheckModel.Code.Equals(resetPasswordCodeManager.GetToken(codeCheckModel.Token)))
			return BadRequest("Wrong password reset token.");

		return Ok(new { });
	}
}
