using GetTeacher.Server.Services.Generators;
using GetTeacher.Server.Services.Managers.Implementations;
using GetTeacher.Server.Services.Managers.Implementations.UserManager;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;

namespace GetTeacher.Server.Extensions.Builder;

public static class GetTeacherBuilderExtensions
{
    public static void UseGetTeacherServices(this WebApplicationBuilder builder)
    {
        builder.UseGetTeacherDb();
        builder.Services.AddScoped<JwtTokenGenerator>();

        // TODO: Why the fuck AddScoped
        builder.Services.AddScoped<ITeacherManager, TeacherManager>();
        builder.Services.AddScoped<IStudentManager, StudentManager>();
        builder.Services.AddScoped<IMeetingMatcher, MeetingMatcher>();
        builder.Services.AddScoped<ITeacherRankManager, TeacherRankManager>();
        builder.Services.AddScoped<IUserStateChecker, UserStateChecker>();
        builder.Services.AddSingleton<IWebSockerManager, Services.Managers.Implementations.Networking.WebSocketManager>();

        // builder.Services.AddSingleton<IMeetingHandler, ?>();
    }
}