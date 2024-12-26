using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Implementations.UserManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Teacher;

[Controller]
[Route("/api/v1/teacher_subjects/add")]
public class AddSubjectToTeacherController(IPrincipalClaimsQuerier principalClaimsQuerier, ITeacherManager teacherManager) : ControllerBase
{
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;
	private readonly ITeacherManager teacherManager = teacherManager;

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> AddSubject([FromBody] TeacherSubjectRequestModel request)
	{
		DbTeacherSubject subject = new() { Subject = new() { Name = request.Name }, 
			Grade = new() { Name = request.Grade} };

		ValidateSubjectTeacher(subject);

		int? teacherUserId = principalClaimsQuerier.GetId(User);
		if (teacherUserId is null)
			return BadRequest(new { });

		DbTeacher? teacher = await teacherManager.GetFromUser(new DbUser { Id = teacherUserId.Value });
		if (teacher is null)
			return BadRequest(new { });

		await teacherManager.AddSubjectToTeacher(subject, teacher);

		return Ok(new { });
	}

	private async void ValidateSubjectTeacher(DbTeacherSubject subject)
	{
		if (!(await teacherManager.GetAllSubjects()).Any(t => t.Name == subject.Subject.Name))
		{
			await teacherManager.AddSubject(subject.Subject);
		}

		if (!(await teacherManager.GetAllGrades()).Where(t => t.Name == subject.Grade.Name).Any())
		{
			await teacherManager.AddGrade(subject.Grade);
		}
	}
}