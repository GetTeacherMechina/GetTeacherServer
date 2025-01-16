using GetTeacher.Server.Models.Subjects;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Subjects;

[Controller]
[Route("/api/v1/subjects")]
public class GetSubjectsController(ISubjectManager subjectManager) : ControllerBase
{
	private readonly ISubjectManager subjectManager = subjectManager;

	[HttpGet]
	public async Task<IActionResult> GetSubjects()
	{
		ICollection<string> subjectNames = (await subjectManager.GetAllSubjects()).Select(s => s.Name).ToList();
		return Ok(new SubjectGetterResposeModel
		{
			Subjects = subjectNames
		});
	}
}