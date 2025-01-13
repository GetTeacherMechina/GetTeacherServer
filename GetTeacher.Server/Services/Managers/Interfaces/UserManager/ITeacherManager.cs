﻿using System.Security.Claims;
using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.UserManager;

public interface ITeacherManager
{
	public Task<DbTeacher?> GetFromUser(ClaimsPrincipal user);

	public Task<bool> TeacherExists(DbUser user);

	public Task<DbTeacher?> GetFromUser(DbUser user);

	public Task<ICollection<DbTeacher>> GetAllTeacher();

	public Task<ICollection<DbTeacher>> GetTeachersBySubjectAndGrade(DbSubject subject, DbGrade garde);

	public Task AddTeacher(DbUser user, DbTeacher teacher);

	public Task RemoveTeacher(DbTeacher teacher);

	public Task<double> GetTeacherRank(DbTeacher teacher);

	public Task<int> GetNumOfTeacherRankers(DbTeacher teacher);

	public Task UpdateTeacherRank(DbTeacher teacher, double newRank);

	public Task IncrementNumOfLessons(DbTeacher teacher);

	public Task<ICollection<DbSubject>> GetAllSubjects();

	public Task AddSubjectToTeacher(DbTeacherSubject subject, DbTeacher teacher);

	public Task AddSubject(DbSubject subject);

	public Task AddGrade(DbGrade garade);

	public Task<ICollection<DbGrade>> GetAllGrades();

	public ICollection<DbTeacherSubject> GetAllTeacherSubjects(DbTeacher teacher);

	public Task RemoveSubjectFromTeacher(DbTeacherSubject subject, DbTeacher teacher);
}