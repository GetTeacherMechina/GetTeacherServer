using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.Chats;

public interface IChatManager
{
    public Task SendToChat(DbChat chat, DbUser self, DbMessage message);

    public Task CreateChat(DbUser self, IEnumerable<DbUser> dbUsers);

}