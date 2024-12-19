using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;

public class DbTeacher
{
	[Key]
	public int Id { get; set; }

	public string Bio { get; set; } = string.Empty;

	[ForeignKey(nameof(DbUser))]
	public int DbUserId { get; set; }
	public virtual DbUser DbUser { get; set; } = null!;

	public double Rank { get; set; }

	public int NumOfLessons { get; set; }

	public virtual ICollection<DbTeacherSubject> TeacherSubjects { get; set; } = [];
}