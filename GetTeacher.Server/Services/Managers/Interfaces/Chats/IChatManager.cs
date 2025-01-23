using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.Chats;

public interface IChatManager
{
	public Task CreateChat(DbUser owner, ICollection<DbUser> participants);

	public Task SendToChat(DbChat chat, DbUser self, DbMessage message);
}