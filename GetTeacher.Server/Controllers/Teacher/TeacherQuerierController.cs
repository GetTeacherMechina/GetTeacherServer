using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Teacher;

[ApiController]
[Route("/api/v1/teachers")]
public class TeacherQuerierController(GetTeacherDbContext getTeacherDbContext) : ControllerBase
{
    private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
		ICollection<DbTeacher> teachers = await getTeacherDbContext.Teachers.Where(t => t != null).ToListAsync();

		return Ok(new
        {
            Teachers = teachers.Select(t =>
			new
			{
				UserId = t.DbUserId,
				t,
			})
		});
    }
}