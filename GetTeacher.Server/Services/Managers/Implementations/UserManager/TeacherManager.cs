﻿using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations.UserManager;

public class TeacherManager(GetTeacherDbContext getTeacherDbContext) : ITeacherManager
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	public async Task AddTeacher(DbUser user, DbTeacher teacher)
	{

		if (await GetFromUser(user) is not null)
			return;

		getTeacherDbContext.Teachers.Add(teacher);
		await getTeacherDbContext.SaveChangesAsync();
	}
	public async Task RemoveTeacher(DbTeacher teacher)
	{
		if (!await TeacherExists(teacher.DbUser))
			return;

		getTeacherDbContext.Teachers.Remove(teacher);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task<ICollection<DbSubject>> GetAllSubjects()
	{
		return await getTeacherDbContext.Subjects.ToListAsync();
	}

	public async Task<ICollection<DbTeacher>> GetAllTeacher()
	{
		return await getTeacherDbContext.Teachers.ToListAsync();
	}

	public async Task<ICollection<DbTeacher>> GetTeachersBySubjectAndGrade(DbSubject subject, DbGrade grade)
	{
		return await getTeacherDbContext.Teachers
			.Include(t => t.DbUser)
			.Include(t => t.TeacherSubjects)
			.ThenInclude(tS => tS.Subject)
			.Include(t => t.TeacherSubjects)
			.ThenInclude(tS => tS.Grade)
			.Where(t => t.TeacherSubjects
				.Any(ts => ts.Subject.Name == subject.Name && ts.Grade.Name == grade.Name))
			.ToListAsync();
	}

	public async Task<DbTeacher?> GetFromUser(DbUser user)
	{
		return await getTeacherDbContext.Teachers.Where(t =>
			t.DbUser == user)
			.Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Subject)
			.Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Grade)
			.FirstOrDefaultAsync();
	}

	public async Task<int> GetNumOfTeacherRankers(DbTeacher teacher)
	{
		if (!await TeacherExists(teacher.DbUser))
			return 0;

		return (await getTeacherDbContext.Teachers.Where(t =>
			t.Id == teacher.Id).FirstAsync()).NumOfLessons;
	}
	public async Task<double> GetTeacherRank(DbTeacher teacher)
	{
		if (!await TeacherExists(teacher.DbUser))
			return 0;

		return (await getTeacherDbContext.Teachers.Where(t =>
			t.Id == teacher.Id).FirstAsync()).Rank;
	}

	public async Task<bool> TeacherExists(DbUser user)
	{
		return await getTeacherDbContext.Teachers.Where(t =>
			t.DbUser.Id == user.Id).FirstOrDefaultAsync() is not null;
	}

	public async Task IncrementNumOfLessons(DbTeacher teacher)
	{
		if (!await TeacherExists(teacher.DbUser))
			return;

		(await getTeacherDbContext.Teachers.Where(t => t.Id == teacher.Id).FirstAsync()).NumOfLessons++;
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task UpdateTeacherRank(DbTeacher teacher, double newRank)
	{
		if (!await TeacherExists(teacher.DbUser))
			return;

		(await getTeacherDbContext.Teachers.Where(t => t.Id == teacher.Id).FirstAsync()).Rank = newRank;
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task AddSubjectToTeacher(DbTeacherSubject subject, DbTeacher teacher)
	{
		if (teacher is null)
		{
			return;
		}
		teacher.TeacherSubjects.Add(subject);

		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task RemoveSubjectFromTeacher(DbTeacherSubject subject, DbTeacher teacher)
	{
		if (teacher is null)
		{
			return;
		}
		teacher.TeacherSubjects.Remove(subject);

		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task AddSubject(DbSubject subject)
	{
		getTeacherDbContext.Subjects.Add(subject);
		await getTeacherDbContext.SaveChangesAsync();
	}


	public async Task<ICollection<DbGrade>> GetAllGrades()
	{
		return await getTeacherDbContext.Grades.ToArrayAsync();
	}

	public async Task AddGrade(DbGrade garade)
	{
		getTeacherDbContext.Grades.Add(garade);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public ICollection<DbTeacherSubject> GetAllTeacherSubjects(DbTeacher teacher)
	{
		return teacher.TeacherSubjects;
	}
}