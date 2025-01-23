using GetTeacher.Server.Models.Profile;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Profile;

[ApiController]
[Route("api/v1/profile")]
public class ProfileController(IStudentReadyTeacherCountNotifier studentReadyTeacherCountNotifier, IUserManager userManager, ITeacherManager teacherManager, IStudentManager studentManager) : ControllerBase
{
	private readonly IStudentReadyTeacherCountNotifier studentReadyTeacherCountNotifier = studentReadyTeacherCountNotifier;
	private readonly IUserManager userManager = userManager;
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly IStudentManager studentManager = studentManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Profile()
	{
		DbUser? user = await userManager.GetFromUser(User);
		if (user is null)
			return BadRequest();

		DbTeacher? teacher = await teacherManager.GetFromUser(user);
		DbStudent? student = await studentManager.GetFromUser(user);

		return Ok(new ProfileResponseModel
		{
			Result = "Success",
			Email = user.Email!,
			FullName = user.UserName!,
			IsStudent = student is not null,
			IsTeacher = teacher is not null,
			Credits = user.Credits,
			Id = user.Id,
		});
	}


	[HttpGet]
	[Authorize]
	[Route("student")]
	public async Task<IActionResult> StudentProfile()
	{
		DbUser? user = await userManager.GetFromUser(User);
		if (user is null)
			return BadRequest();

		DbStudent? student = await studentManager.GetFromUser(user);

		if (student is null)
			return BadRequest();

		await studentReadyTeacherCountNotifier.NotifyStudentsReadyTeachers();
		return Ok(new
		{
			grade = student.Grade
		});
	}
}