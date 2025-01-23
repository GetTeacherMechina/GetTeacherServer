using GetTeacher.Server.Models.Authentication.Register;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class RegisterController(IGradeManager gradeManager, ITwoFactorAuthenticationManager twoFactorAuthenticationManager, ITokenStore tokenStore, UserManager<DbUser> userManager, GetTeacherDbContext getTeacherDbContext) : ControllerBase
{
	private readonly ITwoFactorAuthenticationManager twoFactorAuthenticationManager = twoFactorAuthenticationManager;
	private readonly ITokenStore tokenStore = tokenStore;
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	private readonly IGradeManager gradeManager = gradeManager;

	private async Task AddTeacher(DbUser user, TeacherRequestModel model)
	{
		DbTeacher teacher = new DbTeacher { Bio = model.Bio, DbUserId = user.Id };
		getTeacherDbContext.Add(teacher);
		await getTeacherDbContext.SaveChangesAsync();
	}

	private async Task AddStudent(DbUser user, StudentRequestModel studentRequestModel)
	{
		DbGrade? grade = await getTeacherDbContext.Grades.FirstOrDefaultAsync(g => g.Name == studentRequestModel.Grade);
		if (grade is null)
		{
			grade = new DbGrade { Name = studentRequestModel.Grade };
			await gradeManager.AddGrade(grade);
			await getTeacherDbContext.SaveChangesAsync();
		}

		DbStudent student = new DbStudent { GradeId = grade.Id, DbUserId = user.Id, PriceVsQuality = 50 };
		getTeacherDbContext.Students.Add(student);
		await getTeacherDbContext.SaveChangesAsync();
	}


	[HttpPost]
	public async Task<IActionResult> Register([FromBody] RegisterRequestModel registerModel)
	{
		var user = new DbUser
		{
			UserName = registerModel.FullName,
			Email = registerModel.Email
		};

		if (registerModel.Teacher is null && registerModel.Student is null)
			return BadRequest();

		var result = await userManager.CreateAsync(user, registerModel.Password);

		if (result.Succeeded)
		{
			if (registerModel.Teacher is not null)
				await AddTeacher(user, registerModel.Teacher);

			if (registerModel.Student is not null)
				await AddStudent(user, registerModel.Student);

			// TODO:
			// await twoFactorAuthenticationManager.CreateAndSend2FaCode(user);
			return Ok(new { Message = "2FA needed" });
		}

		foreach (var error in result.Errors)
			ModelState.AddModelError(error.Code, error.Description);

		return BadRequest(ModelState);
	}

	[HttpPost]
	[Route("2fa")]
	public async Task<IActionResult> VerifyEmail2FA([FromBody] VerifyEmail2FaRequestModel verifyEmail2FaRequestModel)
	{
		DbUser? user = await userManager.FindByEmailAsync(verifyEmail2FaRequestModel.Email);
		if (user is null)
			return NotFound("User not found");

		bool success = await twoFactorAuthenticationManager.Confirm2FaCode(user, verifyEmail2FaRequestModel.Code);
		if (success)
			return Ok(new { Status = "Success" });

		return Ok(new { Status = "Invalid code" });
	}
}