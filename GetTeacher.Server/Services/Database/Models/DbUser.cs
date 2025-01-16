using Microsoft.AspNetCore.Identity;

namespace GetTeacher.Server.Services.Database.Models;

public class DbUser : IdentityUser<int>
{
	public double Credits { get; set; }
}