﻿using GetTeacherServer.Models.Profile;
using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacherServer.Controllers.Profile;

[ApiController]
[Route("api/v1/profile")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<DbUser> userManager;

    public ProfileController(UserManager<DbUser> userManager)
    {
        this.userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        DbUser? userResult = await userManager.FindByNameAsync(User.Identity!.Name!);
        if (userResult == null)
            return BadRequest(new ProfileResponseModel { Result = "No such username - wtf authenticated but not found?" });

        return Ok(new ProfileResponseModel { Result = "Success", Email = userResult.Email!, FullName = userResult.UserName! });
    }
}