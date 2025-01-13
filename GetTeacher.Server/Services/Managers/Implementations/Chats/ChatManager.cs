using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.Chats;

namespace GetTeacher.Server.Services.Managers.Implementations.Chats;

public class ChatManager(GetTeacherDbContext getTeacherDbContext) : IChatManager
{
    public async Task SendToChat(DbChat chat, DbUser self, DbMessage message) {
        getTeacherDbContext.Messages.Add(message);
        chat.Messages.Add(message);

        await getTeacherDbContext.SaveChangesAsync();
     }

    public async Task CreateChat(DbUser self, IEnumerable<DbUser> dbUsers)
    {
        getTeacherDbContext.Chats.Add(new()
        {
            Users = dbUsers.Append(self).ToList()
        });

        await getTeacherDbContext.SaveChangesAsync();

    }
}