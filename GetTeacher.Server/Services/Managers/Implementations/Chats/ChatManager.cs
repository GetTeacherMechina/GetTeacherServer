using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.Chats;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace GetTeacher.Server.Services.Managers.Implementations.Chats;

public class ChatManager(GetTeacherDbContext getTeacherDbContext, IWebSocketSystem webSocketSystem) : IChatManager
{
    public async Task SendToChat(DbChat chat, DbUser self, DbMessage message)
    {
        getTeacherDbContext.Messages.Add(message);
        chat.Messages.Add(message);

        await getTeacherDbContext.SaveChangesAsync();
        var tasks = (from user in chat.Users
                     where user.Id != self.Id
                     select webSocketSystem.SendAsync(user.Id, new
                     {
                         message.Id,
                         message.SenderId,
                         message.Content,
                         message.DateTime,
                         SenderName = message.Sender.UserName,
                     }, "chat_message")).ToArray();

        Task.WaitAll(tasks);

    }

    public async Task CreateChat(DbUser self, IEnumerable<DbUser> dbUsers)
    {
        await getTeacherDbContext.Chats.AddAsync(new()
        {
            Users = dbUsers.Append(self).ToList()
        });

        await getTeacherDbContext.SaveChangesAsync();

    }
}