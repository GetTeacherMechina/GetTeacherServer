using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Meeting;

[ApiController]
[Route("api/v1/meeting/favorite-teacher")]
public class AddFavoriteTeacherByMeetingController(GetTeacherDbContext getTeacherDbContext, IStudentManager studentManager) : ControllerBase
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly IStudentManager studentManager = studentManager;

	[HttpPost]
	public async Task<IActionResult> AddTeacherAsFavorite([FromBody] UpdateRankRequestModel updateRankRequestModel)
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		if (student is null)
			return BadRequest();

		DbMeeting? meeting = await getTeacherDbContext.Meetings
			.Where(m => m.Guid == updateRankRequestModel.Guid)
			.Include(m => m.Teacher)
				.ThenInclude(t => t.DbUser)
			.FirstOrDefaultAsync();
		if (meeting is null || meeting.Teacher is null)
			return BadRequest();

		await studentManager.AddFavoriteTeacher(student, meeting.Teacher);
		return Ok(new { });
	}
}