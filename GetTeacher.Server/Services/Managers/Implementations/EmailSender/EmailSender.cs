using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GetTeacher.Server.Services.Managers.Implementations.EmailSender;

public class EmailSender(ILogger<IEmailSender> logger, IConfiguration configuration) : IEmailSender
{
	private readonly ILogger<IEmailSender> logger = logger;
	private readonly IConfiguration configuration = configuration;

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		IConfigurationSection emailSettings = configuration.GetSection("EmailSettings");
		string? smtpServer = emailSettings["SmtpServer"];
		string? smtpPortStr = emailSettings["SmtpPort"];
		string? senderEmail = emailSettings["SenderEmail"];
		string? senderName = emailSettings["SenderName"];
		string? senderPassword = emailSettings["SenderPassword"];

		if (smtpServer is null || smtpPortStr is null || senderEmail is null || senderName is null || senderPassword is null
			|| !int.TryParse(smtpPortStr, out int smtpPort))
		{
			logger.LogCritical("EmailSettings not set up correctly");
			return;
		}

		using var client = new SmtpClient(smtpServer, smtpPort);
		client.Credentials = new NetworkCredential(senderEmail, senderPassword);
		client.EnableSsl = true;

		var mailMessage = new MailMessage
		{
			From = new MailAddress(senderEmail, senderName),
			Subject = subject,
			Body = htmlMessage,
			IsBodyHtml = true,
		};

		mailMessage.To.Add(email);

		await client.SendMailAsync(mailMessage);
	}
}