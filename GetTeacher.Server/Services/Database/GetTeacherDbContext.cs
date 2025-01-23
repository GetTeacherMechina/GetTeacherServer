using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Database;

public class GetTeacherDbContext(DbContextOptions<GetTeacherDbContext> options) : IdentityDbContext<DbUser, IdentityRole<int>, int>(options)
{
	public required DbSet<DbStudent> Students { get; set; }
	public required DbSet<DbTeacher> Teachers { get; set; }
	public required DbSet<DbMeeting> Meetings { get; set; }
	public required DbSet<DbMeetingSummary> MeetingSummaries { get; set; }
	public required DbSet<DbGrade> Grades { get; set; }
	public required DbSet<DbSubject> Subjects { get; set; }
	public required DbSet<DbTeacherSubject> TeacherSubjects { get; set; }
	public required DbSet<DbMessage> Messages { get; set; }
	public required DbSet<DbChat> Chats { get; set; }

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		// TODO: Forward to a background service for efficient non-instant save
		return base.SaveChangesAsync(cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<DbChat>()
			.HasMany(c => c.Users)
			.WithMany(u => u.Chats)
			.UsingEntity<Dictionary<string, object>>(
				"ChatUserRelations",
				u => u.HasOne<DbUser>().WithMany().HasForeignKey("DbUserId"),
				c => c.HasOne<DbChat>().WithMany().HasForeignKey("ChatId")
			);

		modelBuilder.Entity<DbStudent>()
			.HasMany(s => s.FavoriteTeachers)
			.WithMany(t => t.FavoritedByStudents)
			.UsingEntity<Dictionary<string, object>>(
				"StudentTeacherFavorites",
				u => u.HasOne<DbTeacher>().WithMany().HasForeignKey("TeacherId"),
				c => c.HasOne<DbStudent>().WithMany().HasForeignKey("StudentId")
			);
	}
}