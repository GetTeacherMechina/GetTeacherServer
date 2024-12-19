using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;

public class DbStudent
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Grade))]
    public int GradeId { get; set; }
    public virtual DbGrade Grade { get; set; } = null!;


    [ForeignKey(nameof(DbUser))]
    public int DbUserId { get; set; }
    public virtual DbUser DbUser { get; set; } = null!;

    public virtual ICollection<DbTeacher> FavoriteTeachers { get; set; } = [];
    public virtual ICollection<DbLessonSummary> DbLessonSummaries { get; set; } = [];
}