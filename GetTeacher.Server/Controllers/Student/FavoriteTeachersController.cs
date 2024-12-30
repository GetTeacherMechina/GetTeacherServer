using GetTeacher.Server.Models.Student.FavouriteTeacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Student;

[Controller]
[Route("/api/v1/student/[controller]")]
public class FavoriteTeachersController(ITeacherManager teacherManager, IStudentManager studentManager) : Controller
{
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly IStudentManager studentManager = studentManager;

	[HttpPost]
	[Authorize]
	[Route("add")]
	public async Task<IActionResult> AddFavouriteTeacher([FromBody] FavouriteTeacherRequestModel request)
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		DbTeacher? teacher = await teacherManager.GetFromUser(new DbUser { Id = request.TeacherUserId });
		if (student is null || teacher is null)
			return BadRequest();

		await studentManager.AddFavoriteTeacher(student, teacher);
		return Ok(new { });
	}

	[HttpPost]
	[Authorize]
	[Route("remove")]
	public async Task<IActionResult> RemoveFavouriteTeacher([FromBody] FavouriteTeacherRequestModel request)
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		DbTeacher? teacher = await teacherManager.GetFromUser(new DbUser { Id = request.TeacherUserId });
		if (student is null || teacher is null)
			return BadRequest();

		await studentManager.RemoveFavoriteTeacher(student, teacher);
		return Ok(new { });
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetFavouriteTeachers()
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		if (student is null)
			return BadRequest();

		return Ok(new GetFavouriteTeachersResponseModel { FavouriteTeachers = student.FavoriteTeachers });
	}
}