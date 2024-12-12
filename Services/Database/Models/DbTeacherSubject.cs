using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacherServer.Services.Database.Models;

public class DbTeacherSubject
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Subject))]
    public int SubjectId { get; set; }

    [ForeignKey(nameof(Grade))]
    public int GradeId { get; set; }

    public virtual DbSubject Subject { get; set; }

    public virtual DbGrade Grade { get; set; }
}