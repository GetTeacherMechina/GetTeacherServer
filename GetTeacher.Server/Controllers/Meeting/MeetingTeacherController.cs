using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Meeting;

[ApiController]
[Route("api/v1/meeting/teacher")]
public class MeetingTeacherController(GetTeacherDbContext getTeacherDbContext, IPrincipalClaimsQuerier principalClaimsQuerier, ITeacherReadyManager teacherReadyManager) : ControllerBase
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;
	private readonly ITeacherReadyManager teacherReadyManager = teacherReadyManager;

	[Authorize]
	[HttpPost]
	[Route("start")]
	public async Task<IActionResult> TeacherReadyToTeach([FromBody] DbTeacherSubject teacherSubject)
	{
		int? userId = principalClaimsQuerier.GetId(User);
		if (userId is null)
			return BadRequest();

		DbTeacher? teacher = await getTeacherDbContext.Teachers
			.Where(t => t.DbUser.Id == userId.Value)
			.Include(t => t.TeacherSubjects)
				.ThenInclude(tS => tS.Subject) // Include Subject
			.Include(t => t.TeacherSubjects)
				.ThenInclude(tS => tS.Grade)   // Include Grade
			.Include(t => t.DbUser)
			.FirstOrDefaultAsync();
		if (teacher is null)
			return BadRequest();

		teacherReadyManager.ReadyToTeachSubject(teacher, teacherSubject.Subject, teacherSubject.Grade);
		return Ok();
	}

	[Authorize]
	[HttpPost]
	[Route("stop")]
	public async Task<IActionResult> TeacherNotReadyToTeach()
	{
		int? userId = principalClaimsQuerier.GetId(User);
		if (userId is null)
			return BadRequest();

		DbTeacher? teacher = await getTeacherDbContext.Teachers.Where(t => t.DbUser.Id == userId.Value).FirstOrDefaultAsync();
		if (teacher is null)
			return BadRequest();

		teacherReadyManager.NotReadyToTeach(teacher);
		return Ok();
	}
}