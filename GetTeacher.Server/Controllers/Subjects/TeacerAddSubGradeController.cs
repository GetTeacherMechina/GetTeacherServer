using GetTeacher.Server.Models.Subjects.Add;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Subjects;

[Controller]
[Route("/api/v1/subjects/add")]
public class TeacerAddSubGradeController
{
	private GetTeacherDbContext context;
	public TeacerAddSubGradeController(GetTeacherDbContext context)
	{
		this.context = context;
	}


	[HttpPost]
	[Authorize]
	public async Task<IResult> AddSubject([FromBody] AddSubjectRequestModel model)
	{
		DbSubject subjectDb = new() { Name = model.Subject };
		
		try
		{
			await context.AddAsync(subjectDb);
			await context.SaveChangesAsync();
		}
		catch (Exception)
		{
			return Results.BadRequest();
		}

		return Results.Ok();
	}

		
}
