using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Teacher;

[Controller]
[Route("/api/v1/teacher-subjects/remove")]
public class RemoveSubjectFromTeacherController(IPrincipalClaimsQuerier principalClaimsQuerier, ITeacherManager teacherManager) : ControllerBase
{
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;
	private readonly ITeacherManager teacherManager = teacherManager;

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> RemoveSubject([FromBody] TeacherSubjectRequestModel request)
	{
		int? teacherUserId = principalClaimsQuerier.GetId(User);
		if (teacherUserId is null)
			return BadRequest(new { });

		DbTeacher? teacher = await teacherManager.GetFromUser(new DbUser { Id = teacherUserId.Value });
		if (teacher is null)
			return BadRequest(new SubjectTeacherResponsModel());

		DbTeacherSubject? subject = teacher.TeacherSubjects
			.Where(t => t.Subject.Name == request.Name && t.Grade.Name == request.Grade)
			.FirstOrDefault();
		if (subject is null)
			return Ok(new { });

		await teacherManager.RemoveSubjectFromTeacher(subject, teacher);
		return Ok(new { });
	}
}
