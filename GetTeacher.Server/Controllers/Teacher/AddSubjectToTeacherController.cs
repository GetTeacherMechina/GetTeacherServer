using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GetTeacher.Server.Controllers.Teacher
{
	[Controller]
	[Route("/api/v1/subjects/add")]
	public class AddSubjectToTeacherController(ITeacherManager teacherManager, UserManager<DbUser> userManager) : ControllerBase
	{
		private readonly ITeacherManager teacherManager = teacherManager;
		private readonly UserManager<DbUser> userManager = userManager;


		[HttpPost]
		[Authorize]
		public async Task<IActionResult> AddSubject([FromBody] TeacherSubjectRequestModel request)
		{
			DbTeacherSubject subject = new() { Subject = new() { Name = request.Name }, 
				Grade = new() { Name = request.Grade} };

			ValidateSubjectTeacher(subject);

			// ------------------------------------
			var emailClaim = User.FindFirst(ClaimTypes.Email);
			if (emailClaim is null)
			{
				return BadRequest();
			}
			string email = emailClaim.Value;
			DbUser? userResult = await userManager.FindByEmailAsync(email);
			if (userResult is null)
			{
				return BadRequest(new AddSubjectToTeacherResponsModel());
			}
			// ---------------------------------------
			DbTeacher? teacher = await teacherManager.GetFromUser(userResult);
			if (teacher is null)
			{
				return BadRequest(new AddSubjectToTeacherResponsModel());
			}
			await teacherManager.AddSubjectToTeacher(subject, teacher);

			return Ok();
		}

		private async void ValidateSubjectTeacher(DbTeacherSubject subject)
		{
			if ((await teacherManager.GetAllSubjects()).Where(t => t.Name == subject.Subject.Name).Count() == 0)
			{
				await teacherManager.AddSubject(subject.Subject);
			}
			if ((await teacherManager.GetAllGrades()).Where(t => t.Name == subject.Grade.Name).Count() == 0)
			{
				await teacherManager.AddGrade(subject.Grade);
			}
		}
	}
}
