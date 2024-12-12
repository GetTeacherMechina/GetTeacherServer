using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class GetTeacherDbContext : IdentityDbContext<GetTeacherUserIdentity>
{
    public GetTeacherDbContext(DbContextOptions<GetTeacherDbContext> options)
        : base(options)
    {
        
    }
}