using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Teacher;

[Controller]
[Route("/api/v1/teachers")]
public class TeacherQuerierController(GetTeacherDbContext getTeacherDbContext) : ControllerBase
{
    private readonly GetTeacherDbContext db = getTeacherDbContext;

    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
        ICollection<DbTeacher> teachers = await db.Teachers.Include(a=>a.DbUser).ToListAsync();

        return Ok(new
        {
            teachers = teachers.Select((teacher, index) => new
            {
                teacher.Id,
                teacher.DbUser.UserName,
                teacher.Bio,
                teacher.NumOfMeetings,
                teacher.NumOfRankers,
                teacher.Rank,
            })
        });
    }
}