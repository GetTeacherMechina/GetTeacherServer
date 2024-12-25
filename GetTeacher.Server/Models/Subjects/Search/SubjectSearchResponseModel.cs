using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Models.Subjects.Search;

public class SubjectSearchResponseModel
{
    public ICollection<DbSubject> Subjects { get; set; } = new List<DbSubject>();
}
