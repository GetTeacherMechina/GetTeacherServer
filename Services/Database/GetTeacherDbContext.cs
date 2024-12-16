using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class GetTeacherDbContext : IdentityDbContext<DbUser, IdentityRole<int>, int>
{
    public DbSet<DbStudent> Students { get; set; }
    public DbSet<DbTeacher> Teachers { get; set; }
    public DbSet<DbLessonSummary> LessonSummaries { get; set; }
    public DbSet<DbGrade> Grades { get; set; }
    public DbSet<DbSubject> Subjects { get; set; }
    public DbSet<DbTeacherSubject> TeacherSubjects { get; set; }

    public GetTeacherDbContext(DbContextOptions<GetTeacherDbContext> options)
        : base(options)
    {
        
    }
}
