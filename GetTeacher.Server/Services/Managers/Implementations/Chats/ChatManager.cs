using GetTeacher.Server.Models.Chats;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.Chats;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;

namespace GetTeacher.Server.Services.Managers.Implementations.Chats;

public class ChatManager(GetTeacherDbContext getTeacherDbContext, IWebSocketSystem webSocketSystem) : IChatManager
{
	public async Task CreateChat(DbUser user, ICollection<DbUser> participants)
	{
		await getTeacherDbContext.Chats.AddAsync(new DbChat
		{
			Users = [user, .. participants]
		});
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task SendToChat(DbChat chat, DbUser self, DbMessage message)
	{
		chat.Messages.Add(message);

		self.ChatMessagesSent++;
		foreach (DbUser user in chat.Users)
			if (self.Id != user.Id)
				user.ChatMessagesReceived++;

		await getTeacherDbContext.SaveChangesAsync();

		await Task.WhenAll(chat.Users.Select(u =>
			webSocketSystem.SendAsync(u.Id, new SendMessageResponseModel
			{
				Id = message.Id,
				SenderId = message.SenderId,
				Content = message.Content,
				SendTime = message.DateTime,
				SenderName = message.Sender.UserName!,
			}, "chat_message")
		));
	}
}