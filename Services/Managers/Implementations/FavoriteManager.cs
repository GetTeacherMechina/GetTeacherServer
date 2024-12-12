﻿using GetTeacherServer.Services.Managers.Interfaces;

namespace GetTeacherServer.Services.Managers.Implementation;

public class FavoriteManager : IFavoriteManager
{
    private IDb DbM;
    public FavoriteManager(IDb DBM)
    {
        this.DbM = DBM;
    }

    public int[] GetFavoriteTeacherIDs(int studentID)
    {
        return DbM.GetStudentFavoriteTeachers(studentID);
    }

    /**
     * return int[] of the ids of the favorite teacher IDs BySubject and subjectID.
    */
    public int[] GetFavoriteTeacherIDsBySubject(int studentID, int subjectID)
    {
        int[] favoriteTeachers = DbM.GetStudentFavoriteTeachers(studentID);
        int[] subjectTeachers = DbM.GetAllTeacherIDsBySubjectAndStudingLevel(
            DbM.GetStudentStudyingLevelByID(studentID), subjectID);

        int[] favoriteTeacherIDsBySubjectTemp = new int[favoriteTeachers.Length];
        int favoriteTeacherIndex = 0;
        for (int i = 0; i < favoriteTeachers.Length; i++)
        {
            for (int j = 0; j < subjectTeachers.Length; j++)
            {

                if (favoriteTeachers[i] == subjectTeachers[j])
                {
                    favoriteTeacherIDsBySubjectTemp[favoriteTeacherIndex] = favoriteTeachers[i];
                    favoriteTeacherIndex++;
                    break;
                }
            }
        }

        int[] favoriteTeacherIDsBySubject = new int[favoriteTeacherIndex];
        for (int i = 0; i < favoriteTeacherIndex; i++)
        {
            favoriteTeacherIDsBySubject[i] = favoriteTeacherIDsBySubjectTemp[i];
        }

        return favoriteTeacherIDsBySubject;
    }
}
