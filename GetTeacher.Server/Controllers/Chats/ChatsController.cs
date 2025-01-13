using GetTeacher.Server.Models.Chats;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.Chats;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Controllers.Chats;

public class ChatController(GetTeacherDbContext db, IChatManager chatManager, IPrincipalClaimsQuerier principalClaims) : Controller
{




    [Route("/api/v1/chats/create")]
    [HttpPost]
    public async Task<IActionResult> CreateChat([FromBody] ChatCreationModel chat)
    {
        int? id = principalClaims.GetId(User);
        if (id == null)
        {
            return BadRequest("not authenticated");
        }
        DbUser? dbUser = await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        if (dbUser == null)
        {
            return BadRequest("not authenticated");
        }

        DbUser?[] users = await Task.WhenAll(chat.Users.Select(async userid => await db.Users.Where(u => u.Id == userid).FirstOrDefaultAsync()).ToList());
        foreach (var user in users)
        {
            //checking for nulls in the array
            if (user == null)
            {
                return BadRequest("user from user list not found");
            }
        }

        // the null checking happens up here
        await chatManager.CreateChat(dbUser, users.Select(a => a!));
        return Ok(new { });
    }

    [Route("/api/v1/chats/send-message/{chatId}")]
    [HttpPost]
    public async Task<IActionResult> SendMessage(int chatId, [FromBody] MessageCreationModel messageModel)
    {
        int? senderId = principalClaims.GetId(User);
        if (senderId == null)
        {
            return BadRequest("Not authenticated");
        }

        DbUser? sender = await db.Users.FirstOrDefaultAsync(u => u.Id == senderId);
        if (sender == null)
        {
            return BadRequest("Not authenticated");
        }

        DbChat? chat = await db.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null)
        {
            return NotFound("Chat not found");
        }

        if (!chat.Users.Any(u => u.Id == sender.Id))
        {
            return Forbid("You are not a participant in this chat");
        }

        var message = new DbMessage
        {
            Content = messageModel.Content,
            DbUser = sender,
            DbUserId = sender.Id,
            DateTime = DateTime.UtcNow
        };

        await chatManager.SendToChat(chat, sender, message);

        return Ok(new { MessageId = message.Id });
    }

    [Route("/api/v1/chats/{chatId}")]
    [HttpGet]
    public async Task<IActionResult> GetChat(int chatId)
    {
        int? userId = principalClaims.GetId(User);
        if (userId == null)
        {
            return BadRequest("Not authenticated");
        }

        var chat = await db.Chats
            .Include(c => c.Users)
            .Include(c => c.Messages).ThenInclude(m => m.DbUser)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            return NotFound("Chat not found");
        }

        if (!chat.Users.Any(u => u.Id == userId))
        {
            return Forbid("You are not a participant in this chat");
        }


        return Ok(new
        {
            Messages = chat.Messages,
        });
    }

}