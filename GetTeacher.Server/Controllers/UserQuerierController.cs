using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers;

public class UserDetails
{
	public DbUser DbUser { get; set; } = null!;
	public DbStudent? DbStudent { get; set; }
	public DbTeacher? DbTeacher { get; set; }
}

[ApiController]
[Route("/api/v1/users")]
public class UserQuerierController(IUserManager userManager, GetTeacherDbContext getTeacherDbContext) : ControllerBase
{
	private readonly IUserManager userManager = userManager;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetAllUsersExcludingCurrent()
	{
		DbUser? currentUser = await userManager.GetFromUser(User);
		if (currentUser is null)
			return BadRequest("User not found");

		ICollection<UserDetails> users = await (
			from user in getTeacherDbContext.Users
			join student in getTeacherDbContext.Students
				on user.Id equals student.DbUserId into studentGroup
			from student in studentGroup.DefaultIfEmpty() // Left join for optional students
			join teacher in getTeacherDbContext.Teachers
				on user.Id equals teacher.DbUserId into teacherGroup
			from teacher in teacherGroup.DefaultIfEmpty() // Left join for optional teachers
			select new UserDetails
			{
				DbUser = user,
				DbStudent = student,
				DbTeacher = teacher
			}
		).ToListAsync();

		users = users.Where(u => u.DbUser.Id != currentUser.Id).ToList();

		return Ok(new
		{
			Users = users.Select(u => new
			{
				u.DbUser.Id,
				u.DbUser.UserName,
				u.DbTeacher,
				u.DbStudent,
			})
		});
	}
}