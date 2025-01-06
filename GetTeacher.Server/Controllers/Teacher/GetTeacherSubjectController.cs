using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Teacher;

[ApiController]
[Route("api/v1/teacher-subjects")]
public class GetTeacherSubjectController(IPrincipalClaimsQuerier principalClaimsQuerier, ITeacherManager teacherManager) : ControllerBase
{
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;
	private readonly ITeacherManager teacherManager = teacherManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetTeacherSubjects()
	{
		int? teacherUserId = principalClaimsQuerier.GetId(User);
		if (teacherUserId is null)
			return BadRequest(new { });

		DbTeacher? teacher = await teacherManager.GetFromUser(new DbUser { Id = teacherUserId.Value });

		if (teacher is null)
			return BadRequest(new { });

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