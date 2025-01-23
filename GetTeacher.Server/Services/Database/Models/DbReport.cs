using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;

public class DbReport
{
	public int Id { get; set; }

	[ForeignKey(nameof(DbStudent))]
	public int ReporterId { get; set; }
	public virtual DbStudent Reporter { get; set; } = null!;

	public string Content { get; set; } = string.Empty;
}