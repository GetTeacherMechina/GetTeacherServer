using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class GradeManager(ILogger<IGradeManager> logger, GetTeacherDbContext getTeacherDbContext) : IGradeManager
{
	private readonly ILogger<IGradeManager> logger = logger;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	public async Task<ICollection<DbGrade>> GetAllGrades()
	{
		return await getTeacherDbContext.Grades.ToListAsync();
	}

	public async Task AddGrade(DbGrade grade)
	{
		if (getTeacherDbContext.Grades.Any(g => g.Name == grade.Name))
		{
			logger.LogWarning("Grade {gradeName} already exists.", grade.Name);
			return;
		}

		getTeacherDbContext.Grades.Add(grade);
		await getTeacherDbContext.SaveChangesAsync();
	}
}