using GetTeacher.Server.Models.Chats;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.Chats;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Chats;

[ApiController]
[Route("/api/v1/[controller]")]
public class ChatsController(GetTeacherDbContext getTeacherDbContext, IChatManager chatManager, IUserManager userManager) : Controller
{
    private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
    private readonly IChatManager chatManager = chatManager;
    private readonly IUserManager userManager = userManager;

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetChats()
    {
        DbUser? user = await userManager.GetFromUser(User);
        if (user is null)
            return BadRequest("User not found");

        var chats = await getTeacherDbContext.Chats
            .Where(chat => chat.Users.Any(u => u.Id == user.Id)).Include(c => c.Users)
            .ToListAsync();
        return Ok(new
        {
            Chats = chats.Select(chat => new
            {
                chat.Id,
                Users = chat.Users.Select(u => u.UserName).ToList(),
            }).ToList()
        });
    }

    [Authorize]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateChat([FromBody] ChatCreateModelRequest chat)
    {
        DbUser? user = await userManager.GetFromUser(User);
        if (user is null)
            return BadRequest("User not found");

        ICollection<DbUser> participants = await getTeacherDbContext.Users.Where(u => chat.Users.Contains(u.Id)).ToListAsync();
        if (chat.Users.Count != participants.Count)
            return BadRequest("Not all participants found");

        await chatManager.CreateChat(user, participants);
        return Ok(new { });
    }

    [Route("send-message/{chatId}")]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessage(int chatId, [FromBody] MessageCreationModel messageModel)
    {
        DbUser? user = await userManager.GetFromUser(User);
        if (user is null)
            return BadRequest("User not found");

        DbChat? chat = await getTeacherDbContext.Chats
                .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat is null)
            return NotFound("Chat not found");

        if (!chat.Users.Any(u => u.Id == user.Id))
            return Forbid("You are not a participant in this chat");

        var message = new DbMessage
        {
            Content = messageModel.Content,
            Sender = user,
            SenderId = user.Id,
            DateTime = DateTime.UtcNow
        };

        await chatManager.SendToChat(chat, user, message);
        return Ok(new { MessageId = message.Id });
    }

    [Route("{chatId}")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetChat(int chatId)
    {
        DbUser? user = await userManager.GetFromUser(User);
        if (user is null)
            return BadRequest("User not found");

        var chat = await getTeacherDbContext.Chats
                .Include(c => c.Users)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null)
            return NotFound("Chat not found");

        if (!chat.Users.Any(u => u.Id == user.Id))
            return Forbid("You are not a participant in this chat");

        return Ok(new
        {
            Users = chat.Users.Select(u => u.Id),
            Messages = chat.Messages.Select(m => new
            {
                m.Id,
                m.SenderId,
                m.Content,
                m.DateTime,
                SenderName = m.Sender.UserName,
            }).OrderBy(a => a.DateTime),
        });
    }
}