using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Teacher;

[Controller]
[Route("/api/v1/teachers")]
public class TeacherQuerierController(GetTeacherDbContext getTeacherDbContext)
{
    private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

    [HttpGet]
    public async Task<ICollection<DbTeacher>> GetAllTeachers()
    {
        return await getTeacherDbContext.Teachers.ToListAsync();
    }
}