using System.Security.Claims;
using GetTeacher.Server.Models.Profile;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Profile;

[ApiController]
[Route("api/v1/profile")]
public class ProfileController(UserManager<DbUser> userManager, ITeacherManager teacherManager, IStudentManager studentManager) : ControllerBase
{
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly IStudentManager studentManager = studentManager;

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Profile()
	{
		// Shit code
		// TODO: Make claims querier service
		var emailClaim = User.FindFirst(ClaimTypes.Email);
		if (emailClaim is null)
			return BadRequest();

		string email = emailClaim.Value;
		DbUser? userResult = await userManager.FindByEmailAsync(email);
		if (userResult == null)
			return BadRequest(new ProfileResponseModel { Result = "No such username - wtf authenticated but not found?" });

		DbTeacher? teacher = await teacherManager.GetFromUser(userResult);
		DbStudent? student = await studentManager.GetFromUser(userResult);

		return Ok(new ProfileResponseModel
		{
			Result = "Success",
			Email = userResult.Email!,
			FullName = userResult.UserName!,
			IsStudent = student != null,
			IsTeacher = teacher != null
		});
	}
}