using GetTeacher.Server.Models.Subjects.Add;
using GetTeacher.Server.Models.Subjects.AddToTeacher;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Subjects;

[Controller]
[Route("/api/v1/subjects/add_to_teacher")]
public class TeacerAddSubGradeController : ControllerBase
{
	private GetTeacherDbContext context;
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier;

	public TeacerAddSubGradeController(GetTeacherDbContext context, IPrincipalClaimsQuerier principalClaimsQuerier)
	{
		this.context = context;
		this.principalClaimsQuerier = principalClaimsQuerier;
	}


	[HttpPost]
	[Authorize]
	public async Task<IResult> TeacherAddSubGrade([FromBody] TeacherAddSubGradeRequestModel model)
	{
		var subject = await context
			.Subjects
			.Where(s => s.Name==model.Subject)
			.FirstOrDefaultAsync();
		if (subject == null)
		{
			subject = new() { Name = model.Subject };
			await context.AddAsync(subject);
			await context.SaveChangesAsync();	
		}
		var grade = await context.Grades.Where(g => g.Name == model.Grade).FirstAsync();

		var teacherSubject = await context
			.TeacherSubjects
			.Where(ts => ts.SubjectId == subject.Id && ts.GradeId == grade.Id)
			.FirstOrDefaultAsync();

		if (teacherSubject == null)
		{
			teacherSubject = new() { SubjectId = subject.Id, GradeId = grade.Id };
			await context.TeacherSubjects.AddAsync(teacherSubject);
			await context.SaveChangesAsync();
		}

		var teacher = await context
			.Teachers
			.Where(t =>
				t.DbUserId == principalClaimsQuerier.GetId(User)
			).Include(t => t.TeacherSubjects)
			.FirstAsync();
			
		teacher.TeacherSubjects.Add(teacherSubject);
		await context.SaveChangesAsync();

		return Results.Ok();
	}

		
}
