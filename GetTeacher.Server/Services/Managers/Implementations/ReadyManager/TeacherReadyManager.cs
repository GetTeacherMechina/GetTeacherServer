using System.Collections.Concurrent;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;

namespace GetTeacher.Server.Services.Managers.Implementations.ReadyManager;

public class TeacherReadyManager(ITeacherManager teacherManager) : ITeacherReadyManager
{
	private readonly ITeacherManager teacherManager = teacherManager;

	// Teacher.Id -> (Subject.Name, Grade.Name)
	// Using a low level construct id and names because comparing and popping entries by reference sucks
	private static readonly ConcurrentDictionary<int, DbTeacher> readyTeachers = new ConcurrentDictionary<int, DbTeacher>();

	public ICollection<DbTeacher> GetReadyTeachers(DbSubject subject, DbGrade grade)
	{
		return readyTeachers.Where(t =>
			{
				var subjects = teacherManager.GetAllTeacherSubjects(t.Value);
				return subjects.Any(s => s.Subject.Name == subject.Name && s.Grade.Name == grade.Name);
			})
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