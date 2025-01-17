using GetTeacher.Server.Models.Student.Settings;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Student;

[ApiController]
[Route("/api/v1/student/settings")]
public class StudentSettingsController(IStudentManager studentManager) : ControllerBase
{
	private readonly IStudentManager studentManager = studentManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetCurrentSettings()
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		if (student is null)
			return BadRequest();

		return Ok(new StudentSettingsResponseModel
		{
			PriceVsQuality = student.PriceVsQuality
		});
	}

	[HttpPost]
	[Authorize]
	[Route("price-vs-quality")]
	public async Task<IActionResult> SetPriceVsQuality([FromBody] SetPriceVsQualityRequestModel setPriceVsQualityRequestModel)
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		if (student is null)
			return BadRequest();

		await studentManager.SetPriceVsQuality(student, setPriceVsQualityRequestModel.PriceVsQuality);
		return Ok(new { });
	}

}