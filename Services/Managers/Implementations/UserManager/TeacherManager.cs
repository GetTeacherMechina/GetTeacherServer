using GetTeacherServer.Services.Database.Models;
using GetTeacherServer.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacherServer.Services.Managers.Implementations.UserManager
{
    public class TeacherManager : ITeacherManager
    {
        private readonly GetTeacherDbContext getTeacherDbContext;

        public TeacherManager(GetTeacherDbContext getTeacherDbContext)
        {
            this.getTeacherDbContext = getTeacherDbContext;
        }

        public async Task AddTeacher(DbUser user, DbTeacher teacher)
        {
            
            if (await GetFromUser(user) is not null)
            {
                return;
            }
            getTeacherDbContext.Teachers.Add(teacher);
            await getTeacherDbContext.SaveChangesAsync();
        }
        public async Task RemoveTeacher(DbTeacher teacher)
        {
            if (!(await TeacherExists(teacher.DbUser)))
            {
                return;
            }
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

        public async Task<ICollection<DbTeacher>> GetAllTeacherBySubjectAndGrade(DbSubject subject, DbGrade garde)
        {
            return await getTeacherDbContext.Teachers.Where(
                t => t.TeacherSubjects.Where(sub => sub.Subject.Id == subject.Id && 
                sub.Grade.Id == garde.Id).Count() != 0).ToListAsync();
        }

        public async Task<DbTeacher?> GetFromUser(DbUser user)
        {
            return await getTeacherDbContext.Teachers.Where(t =>
                t.DbUser == user).FirstAsync();
        }

        public async Task<int> GetNumOfTeacherRankers(DbTeacher teacher)
        {
            return (await getTeacherDbContext.Teachers.Where(t =>
                t.Id == teacher.Id).FirstAsync()).NumOfLessons;
        }
        public async Task<double> GetTeacherRank(DbTeacher teacher)
        {
            return (await getTeacherDbContext.Teachers.Where(t =>
                t.Id == teacher.Id).FirstAsync()).Rank;
        }

        public async Task<bool> TeacherExists(DbUser user)
        {
            return await getTeacherDbContext.Teachers.Where(t =>
                t.DbUser.Id == user.Id).FirstAsync() is not null;
                
        }

        public async Task UpdateNumOfTeacherRankers(DbTeacher teacher)
        {
            (await getTeacherDbContext.Teachers.Where(t =>
                t.Id == teacher.Id).FirstAsync()).NumOfLessons++;
            await getTeacherDbContext.SaveChangesAsync();
        }

        public async Task UpdateTeacherRank(DbTeacher teacher, double newRank)
        {
            (await getTeacherDbContext.Teachers.Where(t =>
                t.Id == teacher.Id).FirstAsync()).Rank = newRank;
            await getTeacherDbContext.SaveChangesAsync();
        }
    }
}
