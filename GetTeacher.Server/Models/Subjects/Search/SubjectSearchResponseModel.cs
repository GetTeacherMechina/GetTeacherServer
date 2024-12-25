using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Models.Subjects.Search;

public class SubjectSearchResponseModel
{
<<<<<<< HEAD
<<<<<<< HEAD
    public ICollection<DbSubject> Subjects { get; set; } = new List<DbSubject>();
=======
    public ICollection<DbGrade> Subjects { get; set; } = new List<DbGrade>();
>>>>>>> 75765ba (added search subject controller)
=======
    public ICollection<DbSubject> Subjects { get; set; } = new List<DbSubject>();
>>>>>>> 188825a (made response of subject search be properly formatted)
}
