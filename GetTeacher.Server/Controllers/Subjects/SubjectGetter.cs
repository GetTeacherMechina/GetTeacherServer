using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Models.Subjects;

[Controller]
[Route("/api/v1/subjects/search")]
public class subjectGetter(ITeacherManager manger) : ControllerBase
{
	private readonly ITeacherManager manger = manger;

	[HttpGet]
	public async Task<IActionResult> GetSubjects()
	{
		DbSubject[] subjects = (await manger.GetAllSubjects()).ToArray();
		if (subjects == null)
		{
			return BadRequest(new SubjectGetterResposeModel());
		}
		string[] subjectsStr = new string[subjects.Length];
		for (int i = 0; i < subjects.Length; i++) {
			subjectsStr[i] = subjects[i].Name;
		}
		return Ok(new SubjectGetterResposeModel
		{
			Subjects = subjectsStr
		});
	}
}
