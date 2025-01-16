using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Teacher;

[Controller]
[Route("/api/v1/teachers")]
public class TeacherQuerierController(GetTeacherDbContext getTeacherDbContext) : ControllerBase
{
    private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
        var Teachers = await getTeacherDbContext.Teachers.ToListAsync();
        return Ok(new { Teachers });
    }
}