using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Teacher;

[Controller]
[Route("/api/v1/teacher-subjects")]
public class TeacherSubjectController(ITeacherManager teacherManager, ISubjectManager subjectManager, IGradeManager gradeManager) : ControllerBase
{
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly ISubjectManager subjectManager = subjectManager;
	private readonly IGradeManager gradeManager = gradeManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetTeacherSubjects()
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(User);
		if (teacher is null)
			return BadRequest(new { });

		return Ok(new TeacherSubjectsResponseModel
		{
			TeacherSubjects = teacher.TeacherSubjects
		});
	}

	[HttpPost]
	[Authorize]
	[Route("add")]
	public async Task<IActionResult> AddSubject([FromBody] TeacherSubjectRequestModel request)
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(User);
		if (teacher is null)
			return BadRequest(new { });

		DbTeacherSubject teacherSubject = new DbTeacherSubject
		{
			Subject = new DbSubject { Name = request.Subject },
			Grade = new DbGrade { Name = request.Grade }
		};

		Task addSubjectTask = subjectManager.AddSubject(teacherSubject.Subject);
		Task addGradeTask = gradeManager.AddGrade(teacherSubject.Grade);
		await Task.WhenAll(addSubjectTask, addGradeTask);

		await teacherManager.AddSubjectToTeacher(teacherSubject, teacher);

		return Ok(new { });
	}

	[HttpPost]
	[Authorize]
	[Route("remove")]
	public async Task<IActionResult> RemoveSubject([FromBody] TeacherSubjectRequestModel request)
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(User);
		if (teacher is null)
			return BadRequest(new { });

		DbTeacherSubject? teacherSubject = teacher.TeacherSubjects.Where(tS => tS.Subject.Name == request.Subject && tS.Grade.Name == request.Grade).FirstOrDefault();
		if (teacherSubject is null)
			return BadRequest(new { Error = "You're not teaching that subject." });

		await teacherManager.RemoveSubjectFromTeacher(teacherSubject, teacher);

		return Ok(new { });
	}
}