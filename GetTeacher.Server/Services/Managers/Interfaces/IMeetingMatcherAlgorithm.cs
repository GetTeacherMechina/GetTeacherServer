﻿using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IMeetingMatcherAlgorithm
{
	/*
     Look for a teacher based on the selected subject and the student's studying level.
     If no such teacher is available, call NotifyOnlineTeachers and wait for a connection.
     Input student id, subject id.
     Output selected teacher id.
    */
	public Task<DbTeacher?> MatchStudentTeacher(DbStudent student, DbSubject subject, ICollection<DbTeacher> teacherExclusions);
}