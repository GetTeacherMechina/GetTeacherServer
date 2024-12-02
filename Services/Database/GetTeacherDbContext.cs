using Microsoft.EntityFrameworkCore;

public class GetTeacherDbContext : DbContext
{
    public GetTeacherDbContext(DbContextOptions<GetTeacherDbContext> options)
     : base(options)
    {

    }
}