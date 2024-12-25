﻿
using GetTeacher.Server.Models.Subjects.Search;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GetTeacher.Server.Controllers.Subjects;

[Controller]
[Route("/api/v1/subjects/search")]
public class SubjectSearchController: ControllerBase
{
    private GetTeacherDbContext context;
    public SubjectSearchController(GetTeacherDbContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IActionResult> SearchSubjects([FromBody] SubjectSearchRequestModel request)
    {
		Console.WriteLine($"%{request.SubjectName}%");
        var subjects = context
            .Subjects
            .Where(s => EF.Functions.Like(s.Name, $"%{request.SubjectName}%"));
        if (subjects == null)
        {
            return Ok(new SubjectSearchResponseModel());
        }


        return Ok(await subjects.ToListAsync());
    }


}
