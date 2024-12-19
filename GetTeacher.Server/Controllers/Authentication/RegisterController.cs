using GetTeacher.Server.Models.Authentication.Register;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class RegisterController(UserManager<DbUser> userManager, GetTeacherDbContext getTeacherDbContext) : ControllerBase
{
	private readonly UserManager<DbUser> userManager = userManager;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	private async Task AddTeacher(DbUser user, TeacherRequestModel model)
	{
		DbTeacher teacher = new DbTeacher { Bio = model.Bio, DbUserId = user.Id };
		getTeacherDbContext.Add(teacher);
		await getTeacherDbContext.SaveChangesAsync();
	}

	private async Task AddStudent(DbUser user, StudentRequestModel model)
	{
		var grade = await getTeacherDbContext.Grades
			   .FirstOrDefaultAsync(g => g.Name == model.Grade);

		if (grade == null)
		{
			grade = new DbGrade { Name = model.Grade };
			getTeacherDbContext.Grades.Add(grade);
			await getTeacherDbContext.SaveChangesAsync();
		}

		DbStudent student = new DbStudent { GradeId = grade.Id, DbUserId = user.Id };
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

		var result = await userManager.CreateAsync(user, registerModel.Password);

		if (result.Succeeded)
		{
			if (registerModel.Teacher is not null)
				await AddTeacher(user, registerModel.Teacher);

			if (registerModel.Student is not null)
				await AddStudent(user, registerModel.Student);

			return Ok(new { Message = "Registration successful" });
		}

		foreach (var error in result.Errors)
			ModelState.AddModelError(error.Code, error.Description);

		return BadRequest(ModelState);
	}
}