using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacherServer.Services.Database.Models;

public class DbTeacher
{
    [Key]
    public int Id { get; set; }

    public string Bio { get; set; }

    [ForeignKey(nameof(DbUser))]
    public int DbUserId { get; set; }

    public virtual DbIdentityUser DbUser { get; set; }

    public virtual ICollection<DbTeacherSubject> TeacherSubjects { get; set; } = new List<DbTeacherSubject>();
}