using System.Collections.Concurrent;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class TeacherReadyManager : ITeacherReadyManager
{
	// Teacher.Id -> (Subject.Name, Grade.Name)
	// Using a low level construct id and names because comparing and popping entries by reference sucks
	private static readonly ConcurrentDictionary<int, (DbTeacher teacher, string subName, string gradeName)> readyTeachers = new ConcurrentDictionary<int, (DbTeacher teacher, string, string)>();

	public ICollection<DbTeacher> GetReadyTeachers(DbSubject subject, DbGrade grade)
	{
		return readyTeachers
			.Where(t => t.Value.subName == subject.Name && t.Value.gradeName == grade.Name)
			.Select(t => t.Value.teacher)
			.ToList();
	}

	public void ReadyToTeachSubject(DbTeacher teacher, DbSubject subject, DbGrade grade)
	{
		readyTeachers.AddOrUpdate(teacher.Id,
			(teacher, subject.Name, grade.Name),
			(key, value) => (teacher, subject.Name, grade.Name));
	}
	public void NotReadyToTeach(DbTeacher teacher)
	{
		readyTeachers.Remove(teacher.Id, out _);
	}
}