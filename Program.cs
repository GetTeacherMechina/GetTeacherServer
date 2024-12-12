using GetTeacherServer.BuilderExtensions;
using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GetTeacherServer;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Test");
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        // Adds the custom services to the service collection
        builder.UseGetTeacherServices();

        builder.AddJwtAuthentication();

        // Adds the identity services
        builder.Services.AddIdentityCore<DbIdentityUser>()
            .AddEntityFrameworkStores<GetTeacherDbContext>()
            .AddDefaultTokenProviders();

        var app = builder.Build();

        // Redirects http requests to https
        app.UseHttpsRedirection();

        // Authentication pipeline stage happens before the authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Run duh
        app.Run();
    }
}