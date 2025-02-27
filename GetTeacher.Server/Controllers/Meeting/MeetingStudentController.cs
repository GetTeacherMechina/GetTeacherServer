﻿using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Meeting;

[ApiController]
[Route("api/v1/meeting/student")]
public class MeetingStudentController(IStudentManager studentManager, ISubjectManager subjectManager, IStudentReadyManager studentReadyManager, IUserStateTracker userStateTracker, GetTeacherDbContext getTeacherDbContext, IMeetingMatcherBackgroundService meetingMatcherBackgroundService, IPrincipalClaimsQuerier principalClaimsQuerier) : ControllerBase
{
	private readonly IStudentManager studentManager = studentManager;
	private readonly ISubjectManager subjectManager = subjectManager;
	private readonly IStudentReadyManager studentReadyManager = studentReadyManager;
	private readonly IUserStateTracker userStateTracker = userStateTracker;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly IMeetingMatcherBackgroundService meetingMatcherBackgroundService = meetingMatcherBackgroundService;
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;

	[Authorize]
	[HttpPost]
	[Route("start")]
	public async Task<IActionResult> StartSearching([FromBody] DbSubject subject)
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		if (student is null)
			return BadRequest();

		if (student.DbUser.Credits <= 0)
			return BadRequest();

		DbSubject? dbSubject = await subjectManager.GetFromName(subject.Name);
		if (dbSubject is null)
			return BadRequest();

		userStateTracker.AddDisconnectAction(student, (i) => studentReadyManager.NotReadyToStart(student));
		studentReadyManager.ReadyToStart(student, dbSubject);
		return Ok(new { });
	}

	[Authorize]
	[HttpPost]
	[Route("Stop")]
	public async Task<IActionResult> StopSearching()
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

		userStateTracker.ClearDisconnectActions(student);
		meetingMatcherBackgroundService.StopMatchStudent(student);
		return Ok(new { });
	}
}