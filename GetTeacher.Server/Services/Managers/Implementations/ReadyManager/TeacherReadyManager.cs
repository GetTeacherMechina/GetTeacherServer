using System.Collections.Concurrent;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations.ReadyManager;

public class TeacherReadyManager(GetTeacherDbContext getTeacherDbContext, ITeacherManager teacherManager) : ITeacherReadyManager
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly ITeacherManager teacherManager = teacherManager;

	// Teacher.Id -> (Subject.Name, Grade.Name)
	// Using a low level construct id and names because comparing and popping entries by reference sucks
	private static readonly ConcurrentDictionary<int, DbTeacher> readyTeachers = new ConcurrentDictionary<int, DbTeacher>();

	public async Task<ICollection<SubjectReadyTeachersDescriptor>> GetReadyTeachersDescriptors()
	{
		ICollection<DbSubject> subjects = await getTeacherDbContext.Subjects.ToListAsync();
		ICollection<DbGrade> grades = await getTeacherDbContext.Grades.ToListAsync();

		ICollection<SubjectReadyTeachersDescriptor> subjectReadyTeachersDescriptors = [];
		foreach (DbSubject subject in subjects)
			foreach (DbGrade grade in grades)
				subjectReadyTeachersDescriptors.Add(new SubjectReadyTeachersDescriptor(subject, grade, GetReadyTeachersForSubjectAndGrade(subject, grade).Count));

		return subjectReadyTeachersDescriptors;
	}

	public ICollection<DbTeacher> GetReadyTeachersForSubjectAndGrade(DbSubject subject, DbGrade grade)
	{
		return readyTeachers
			.Where(t => teacherManager.GetAllTeacherSubjects(t.Value)
						.Any(s => s.Subject.Name == subject.Name && s.Grade.Name == grade.Name))
			.Select(t => t.Value)
			.ToList();
	}

	public void ReadyToTeachSubject(DbTeacher teacher)
	{
		readyTeachers.AddOrUpdate(teacher.Id,
			teacher,
			(key, value) => teacher);

	}

	public void NotReadyToTeach(DbTeacher teacher)
	{
		readyTeachers.Remove(teacher.Id, out _);
	}
}