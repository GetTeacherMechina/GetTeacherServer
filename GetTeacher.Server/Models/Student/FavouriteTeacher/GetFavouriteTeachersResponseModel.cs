using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Models.Student.FavouriteTeacher;

public class GetFavouriteTeachersResponseModel
{
	public ICollection<DbTeacher> FavouriteTeachers { get; set; } = [];
}
