using GetTeacher.Server.Models.Teacher.Settings;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Teacher;

[ApiController]
[Route("/api/v1/teacher/settings")]
public class TeacherSettingsController(ITeacherManager teacherManager) : ControllerBase
{
	private readonly ITeacherManager teacherManager = teacherManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetCurrentSettings()
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(User);
		if (teacher is null)
			return BadRequest();

		return Ok(new TeacherSettingsResponseModel
		{
			TariffPerMinute = teacher.TariffPerMinute,
			Bio = teacher.Bio
		});
	}

	[HttpPost]
	[Authorize]
	[Route("tariff")]
	public async Task<IActionResult> SetCreditsTariff([FromBody] SetCreditsTariffRequestModel setCreditsTariffRequestModel)
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(User);
		if (teacher is null)
			return BadRequest();

		await teacherManager.SetCreditsTariff(teacher, setCreditsTariffRequestModel.CreditsTariffPerMinute);
		return Ok(new { });
	}

	[HttpPost]
	[Authorize]
	[Route("bio")]
	public async Task<IActionResult> SetCreditsTariff([FromBody] SetBioRequestModel setCreditsTariffRequestModel)
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(User);
		if (teacher is null)
			return BadRequest();

		await teacherManager.SetBio(teacher, setCreditsTariffRequestModel.Bio);
		return Ok(new { });
	}
}