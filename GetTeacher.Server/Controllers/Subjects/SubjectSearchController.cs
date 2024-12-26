using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Models.Subjects;

[Controller]
[Route("/api/v1/subjects/search")]
public class SubjectSearchController(ITeacherManager teacherManager) : ControllerBase
{
	private readonly ITeacherManager teacherManager = teacherManager;

	[HttpGet]
	public async Task<IActionResult> GetSubjects()
	{
		DbSubject[] subjects = (await teacherManager.GetAllSubjects()).ToArray();
		if (subjects is null)
		{
			return BadRequest(new SubjectGetterResposeModel());
		}

		string[] subjectsStr = new string[subjects.Length];
		for (int i = 0; i < subjects.Length; i++)
		{
			subjectsStr[i] = subjects[i].Name;
		}

		return Ok(new SubjectGetterResposeModel
		{
			Subjects = subjectsStr
		});
	}
}