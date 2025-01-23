using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class TwoFactorAuthenticationManager(ITokenStore tokenStore, IEmailSender emailSender, UserManager<DbUser> userManager) : ITwoFactorAuthenticationManager
{
	private readonly ITokenStore tokenStore = tokenStore;
	private readonly IEmailSender emailSender = emailSender;
	private readonly UserManager<DbUser> userManager = userManager;

	public async Task CreateAndSend2FaCode(DbUser user)
	{
		var twoFactorToken = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
		string code = tokenStore.CreateCodeForToken(twoFactorToken);
		await emailSender.SendEmailAsync(user.Email!, "Your Two-Factor Authentication Code", $"Your 2FA code is: {code}");
	}

	public async Task<bool> Confirm2FaCode(DbUser user, string code)
	{
		string? token = tokenStore.GetToken(code);
		if (token is null)
			return false;

		var isValid = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, token);

		if (isValid)
		{
			tokenStore.RemoveCode(code);
			user.EmailConfirmed = true;
			await userManager.UpdateAsync(user);
			return true;
		}

		return false;
	}
}