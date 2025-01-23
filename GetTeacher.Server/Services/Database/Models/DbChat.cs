namespace GetTeacher.Server.Services.Database.Models;

public class DbChat
{
	public int Id { get; set; }

	public virtual ICollection<DbUser> Users { get; set; } = [];

	public virtual ICollection<DbMessage> Messages { get; set; } = [];
}