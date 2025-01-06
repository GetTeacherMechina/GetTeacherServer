using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GetTeacher.Server.Controllers.Teacher;

[ApiController]
[Route("api/v1/teacher_subjects")]
public class GetTheacherSubjectController(ITeacherManager teacherManager, UserManager<DbUser> userManager) : ControllerBase
{
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly UserManager<DbUser> userManager = userManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetTeacherSubjects()
	{
		DbTeacher? teacher = await Utils.GetTeacherFromUser(User, userManager, teacherManager);

		if (teacher is null)
		{
			return BadRequest(new AddSubjectToTeacherResponsModel());
		}
		DbTeacherSubject[] teacherSubjects = teacher.TeacherSubjects.ToArray();

		string[][] teacherSubjectsStr = ExtrctingTeacherSubjects(teacherSubjects);

		return Ok(new AddSubjectToTeacherResponsModel 
		{
			Grades = teacherSubjectsStr[0],
			Subjects = teacherSubjectsStr[1]
		});
	}

	//**
	//return subject in index 1 and grades in index 0
	//*/
	private string[][] ExtrctingTeacherSubjects(DbTeacherSubject[] teacherSubjects)
	{
		string[] subject = new string[teacherSubjects.Length];
		string[] grades = new string[teacherSubjects.Length];
		for (int i = 0; i < teacherSubjects.Length; i++)
		{
			subject[i] = teacherSubjects[i].Subject.Name;
			grades[i] = teacherSubjects[i].Grade.Name;
		}
		return new string[][] { grades , subject };
	}


}