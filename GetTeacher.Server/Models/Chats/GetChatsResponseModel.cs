using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Models.Chats;

public class GetChatsResponseModel
{
	public ICollection<DbChat> Chats { get; set; } = [];
}