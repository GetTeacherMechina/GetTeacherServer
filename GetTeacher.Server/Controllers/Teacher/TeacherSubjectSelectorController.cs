using GetTeacher.Server.Models.Teacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GetTeacher.Server.Controllers.Teacher
{
	[ApiController]
	[Route("api/v1/TeacherSubjectSelectorController")]
	public class TeacherSubjectSelectorController : ControllerBase
	{
		private readonly ITeacherManager teacherManager;
		private readonly UserManager<DbUser> userManager;

		public TeacherSubjectSelectorController(ITeacherManager teacherManager, UserManager<DbUser> userManager)
		{
			this.teacherManager = teacherManager;
			this.userManager = userManager;
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetTeacherSubjects()
		{
			var emailClaim = User.FindFirst(ClaimTypes.Email);
			if (emailClaim is null)
				return BadRequest();

			string email = emailClaim.Value;
			DbUser? userResult = await userManager.FindByEmailAsync(email);
			if (userResult == null)
			{
				return BadRequest(new TeacherSubjectsResponsModel());
			}
			DbTeacherSubject[] teacherSubjects = (await teacherManager.GetFromUser(userResult)).TeacherSubjects.ToArray();
			String[] subject = new string[teacherSubjects.Length];
			String[] grades = new string[teacherSubjects.Length];
			for (int i = 0; i < teacherSubjects.Length; i++)
			{
				subject[i] = teacherSubjects[i].Subject.Name;
				grades[i] = teacherSubjects[i].Grade.Name;
			}
			return Ok(new TeacherSubjectsResponsModel 
			{
				Grades = grades,
				Subjects = subject
			});
		}
	}

}