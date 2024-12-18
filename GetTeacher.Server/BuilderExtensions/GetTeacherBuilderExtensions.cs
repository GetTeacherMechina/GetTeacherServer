using GetTeacherServer.Services.Generators;
using GetTeacherServer.Services.Managers.Implementation;
using GetTeacherServer.Services.Managers.Implementations;
using GetTeacherServer.Services.Managers.Implementations.UserManager;
using GetTeacherServer.Services.Managers.Interfaces;
using GetTeacherServer.Services.Managers.Interfaces.UserManager;

namespace GetTeacherServer.BuilderExtensions;

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

        // builder.Services.AddSingleton<IMeetingHandler, ?>();
    }
}