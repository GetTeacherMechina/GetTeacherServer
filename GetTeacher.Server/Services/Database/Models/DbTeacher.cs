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

	public int NumOfRankers { get; set; }

	public double Rank { get; set; }

	public int NumOfMeetings { get; set; }

	public double TariffPerMinute { get; set; }

	public ICollection<string> Reports { get; set; } = new List<string>();

	public virtual ICollection<DbTeacherSubject> TeacherSubjects { get; set; } = [];

	public virtual ICollection<DbStudent> FavoritedByStudents { get; set; } = [];
}