using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacherServer.Services.Database.Models;

public class DbTeacher
{
    [Key]
    public int Id { get; set; }

    public string Bio { get; set; }

    [ForeignKey(nameof(DbUser))]
    public string DbUserId { get; set; }
    public virtual DbUser DbUser { get; set; }

    public double Rank { get; set; }

    public int NumOfLessons { get; set; }

    public virtual ICollection<DbTeacherSubject> TeacherSubjects { get; set; } = new List<DbTeacherSubject>();
}