using GetTeacherServer.Models.Authentication.Register;
using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacherServer.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly UserManager<DbUser> userManager;
    private readonly GetTeacherDbContext context;

    public RegisterController(UserManager<DbUser> userManager, GetTeacherDbContext context)
    {
        this.userManager = userManager;
        this.context = context;
    }



    private async Task AddTeacher(DbUser user, TeacherRequestModel model)
    {
        DbTeacher teacher = new DbTeacher { Bio = model.Bio, DbUserId = user.Id };
        context.Add(teacher);
        await context.SaveChangesAsync();
    }

    private async Task AddStudent(DbUser user, StudentRequestModel model)
    {
        var grade = await context.Grades
               .FirstOrDefaultAsync(g => g.Name == model.Grade);

        if (grade == null)
        {
            grade = new DbGrade { Name = model.Grade };
            context.Grades.Add(grade);
            await context.SaveChangesAsync();
        }

        DbStudent student = new DbStudent { GradeId = grade.Id, DbUserId = user.Id };
        context.Students.Add(student);
        await context.SaveChangesAsync();
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
            {
                await AddTeacher(user, registerModel.Teacher);
            }

            if (registerModel.Student is not null)
            {
                await AddStudent(user, registerModel.Student);
            }

            return Ok(new { Message = "Registration successful" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return BadRequest(ModelState);
    }
}