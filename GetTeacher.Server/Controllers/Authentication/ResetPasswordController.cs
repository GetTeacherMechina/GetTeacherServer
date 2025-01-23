using GetTeacher.Server.Models.Profile;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Authentication;

[Controller]
[Route("/api/v1/auth/reset-password")]
public class ResetPasswordController(UserManager<DbUser> userManager, IEmailSender emailSender, ITokenStore tokenStore) : ControllerBase
{
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly IEmailSender emailSender = emailSender;
	private readonly ITokenStore resetPasswordTokenStore = tokenStore;

	[HttpPost]
	[Route("forgot")]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel forgotPasswordRequestModel)
	{
		DbUser? user = await userManager.FindByEmailAsync(forgotPasswordRequestModel.Email);
		if (user is null)
			return BadRequest("No such email found.");

		string token = await userManager.GeneratePasswordResetTokenAsync(user);
		string code = resetPasswordTokenStore.CreateCodeForToken(token);
		await emailSender.SendEmailAsync(user.Email!, "Reset Password", $"Please reset your password by entering this code: {code}");
		return Ok(new { });
	}

	[HttpPost]
	[Route("reset")]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel resetPasswordRequestModel)
	{
		DbUser? user = await userManager.FindByEmailAsync(resetPasswordRequestModel.Email);
		if (user is null)
			return BadRequest("No such email found.");

		if (resetPasswordRequestModel.Password != resetPasswordRequestModel.ConfirmPassword)
			return BadRequest("Passwords do not match.");

		string? token = resetPasswordTokenStore.GetToken(resetPasswordRequestModel.Code);
		if (token is null)
			return BadRequest("Token not found.");

		IdentityResult result = await userManager.ResetPasswordAsync(user, token, resetPasswordRequestModel.Password);
		if (!result.Succeeded)
			return BadRequest(result.ToString());

		resetPasswordTokenStore.RemoveCode(resetPasswordRequestModel.Code);
		return Ok(new { });
	}
}