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




    [Route("/api/v1/teacher-subjects/add")]
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
}