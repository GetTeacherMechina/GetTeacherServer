using GetTeacherServer.Models.Profile;
using GetTeacherServer.Services.Database.Models;
using GetTeacherServer.Services.Managers.Implementations.UserManager;
using GetTeacherServer.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacherServer.Controllers.Profile;

[ApiController]
[Route("api/v1/profile")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<DbUser> userManager;
    private readonly ITeacherManager iTeacherManager;
    private readonly IStudentManager iStudentManager;

    public ProfileController(UserManager<DbUser> userManager, GetTeacherDbContext context)
    {
        this.userManager = userManager;
        iTeacherManager = new TeacherManager(context);
        iStudentManager = new StudentManager(context);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        DbUser? userResult = await userManager.FindByNameAsync(User.Identity!.Name!);
        if (userResult == null)
            return BadRequest(new ProfileResponseModel { Result = "No such username - wtf authenticated but not found?" });
        DbTeacher? teacher = await iTeacherManager.GetFromUser(userResult);
        DbStudent? student = await iStudentManager.GetFromUser(userResult);

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