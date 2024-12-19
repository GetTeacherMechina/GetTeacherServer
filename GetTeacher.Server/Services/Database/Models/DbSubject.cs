using System.ComponentModel.DataAnnotations;

namespace GetTeacher.Server.Services.Database.Models;

public class DbSubject
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Name { get; set; } = string.Empty;
}