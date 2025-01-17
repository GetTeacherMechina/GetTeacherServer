using Microsoft.AspNetCore.Identity;

namespace GetTeacher.Server.Services.Database.Models;

public class DbUser : IdentityUser<int>
{
	public double Credits { get; set; }

	public int ChatMessagesSent { get; set; }

	public int ChatMessagesReceived { get; set; }
}