using GetTeacherServer.BuilderExtensions;
using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace GetTeacherServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        // Adds the custom services to the service collection
        builder.UseGetTeacherServices();

        builder.AddJwtAuthentication();

        // Adds the identity services
        builder.Services.AddIdentityCore<DbUser>()
            .AddEntityFrameworkStores<GetTeacherDbContext>()
            .AddDefaultTokenProviders();
        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            //TODO: actually manage cors
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        app.UseCors();
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