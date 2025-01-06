using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GetTeacher.Server.Controllers.Teacher;

[Controller]
[Route("/api/v1/teacher_subjects/remove")]
public class RemoveSubjectFromTeacher(ITeacherManager teacherManager, UserManager<DbUser> userManager) : ControllerBase
{
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly UserManager<DbUser> userManager = userManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> RemoveSubject([FromBody] TeacherSubjectRequestModel request)
	{
		DbTeacher? teacher = await Utils.GetTeacherFromUser(User, userManager, teacherManager);

		if (teacher is null)
		{
			return BadRequest(new AddSubjectToTeacherResponsModel());
		}

		 DbTeacherSubject? subject = teacher.TeacherSubjects.Where(t => 
			t.Subject.Name == request.Name && t.Grade.Name == request.Grade).FirstOrDefault();
		if (subject is null) {
			return Ok();
		}
		await teacherManager.RemoveSubjectFromTeacher(subject, teacher);
		return Ok();
	}

}
