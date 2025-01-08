using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace GetTeacher.Server.Services.Managers.Implementations.EmailSender;

public class IdentityEmailSender : IEmailSender<DbUser>
{
	public Task SendConfirmationLinkAsync(DbUser user, string email, string confirmationLink)
	{
		throw new NotImplementedException();
	}

	public Task SendPasswordResetCodeAsync(DbUser user, string email, string resetCode)
	{
		throw new NotImplementedException();
	}

	public Task SendPasswordResetLinkAsync(DbUser user, string email, string resetLink)
	{
		throw new NotImplementedException();
	}
}