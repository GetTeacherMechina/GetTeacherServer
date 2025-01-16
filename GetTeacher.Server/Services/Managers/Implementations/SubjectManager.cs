using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class SubjectManager(ILogger<ISubjectManager> logger, GetTeacherDbContext getTeacherDbContext) : ISubjectManager
{
	private readonly ILogger<ISubjectManager> logger = logger;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	public async Task<ICollection<DbSubject>> GetAllSubjects()
	{
		return await getTeacherDbContext.Subjects.ToListAsync();
	}

	public async Task AddSubject(DbSubject subject)
	{
		if (getTeacherDbContext.Subjects.Any(s => s.Name == subject.Name))
		{
			logger.LogWarning("Subject {subjectName} already exists.", subject.Name);
			return;
		}

		// TODO: Notify all students that a new subject is available and that they should re-query the subject list
		getTeacherDbContext.Subjects.Add(subject);
		await getTeacherDbContext.SaveChangesAsync();
	}
}