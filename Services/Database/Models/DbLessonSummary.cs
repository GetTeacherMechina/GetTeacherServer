using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacherServer.Services.Database.Models;

public class DbLessonSummary
{
    public int Id { get; set; }

    [ForeignKey(nameof(Teacher))]
    public int TeacherId { get; set; }

    public virtual DbTeacher Teacher { get; set; }

    [ForeignKey(nameof(Student))]
    public int StudentId { get; set; }

    public virtual DbStudent Student { get; set; }

    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}