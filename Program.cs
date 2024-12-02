using GetTeacherServer.BuilderExtensions;
using Microsoft.EntityFrameworkCore;

namespace GetTeacherServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.UseGetTeacherDb();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}