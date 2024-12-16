﻿using GetTeacherServer.Services.Database.Models;
using GetTeacherServer.Services.Managers.Interfaces;
using GetTeacherServer.Services.Managers.Interfaces.UserManager;

namespace GetTeacherServer.Services.Managers.Implementations;

public class TeacherRankManager : ITeacherRankManager
{
    private readonly ITeacherManager teacherManager;
    private readonly IStudentManager studentManager;

    public TeacherRankManager(ITeacherManager teacherManager, IStudentManager studentManager)
    {
        this.teacherManager = teacherManager;
        this.studentManager = studentManager;
    }

    public async Task<ICollection<DbTeacher>> GetRankedTeachersBySubjectAndGradeAndFavorite(DbStudent student, DbSubject subject, DbGrade grade)
    {
        ICollection<DbTeacher> teachers = await teacherManager.GetTeachersBySubjectAndGrade(subject, grade);
        ICollection<DbTeacher> favoriteTeachers = student.FavoriteTeachers;

        teachers.OrderByDescending(t => t.Rank);
        favoriteTeachers.OrderByDescending(t => t.Rank);

        teachers = teachers.Except(favoriteTeachers).ToList();

        return favoriteTeachers.Concat(teachers).ToList();
    }

    public async Task UpdateRank(DbUser teacherUser, int stars)
    {
        DbTeacher? teacher = await teacherManager.GetFromUser(teacherUser);
        if (teacher is null)
            return;

        double currentRank = await teacherManager.GetTeacherRank(teacher);
        int numOfRankers = await teacherManager.GetNumOfTeacherRankers(teacher);
        double newRank = ((currentRank * numOfRankers) + stars) / (numOfRankers + 1);

        await teacherManager.UpdateTeacherRank(teacher, newRank);
        await teacherManager.IncrementNumOfLessons(teacher);
    }
}