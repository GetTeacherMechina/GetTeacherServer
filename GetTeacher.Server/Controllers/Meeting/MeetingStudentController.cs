using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Meeting;

[ApiController]
[Route("api/v1/meeting/student")]
public class MeetingStudentController(GetTeacherDbContext getTeacherDbContext, IMeetingMatcherBackgroundService meetingMatcherBackgroundService, IPrincipalClaimsQuerier principalClaimsQuerier) : ControllerBase
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly IMeetingMatcherBackgroundService meetingMatcherBackgroundService = meetingMatcherBackgroundService;
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;

	[Authorize]
	[HttpPost]
	[Route("start")]
	public async Task<IActionResult> StartSearching([FromBody] DbSubject subject)
	{
		int? userId = principalClaimsQuerier.GetId(User);
		if (userId is null)
			return BadRequest();

		DbStudent? student = await getTeacherDbContext.Students
			.Where(s => s.DbUser.Id == userId.Value)
			.Include(s => s.DbUser)
			.Include(s => s.Grade)
			.FirstOrDefaultAsync();
		if (student is null)
			return BadRequest();

		meetingMatcherBackgroundService.MatchStudent(student, subject);
		return Ok();
	}
}