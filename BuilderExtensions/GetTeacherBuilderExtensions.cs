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

        // TODO: Why the fuck singletons
        builder.Services.AddSingleton<ITeacherManager, TeacherManager>();
        builder.Services.AddSingleton<IStudentManager, StudentManager>();
        builder.Services.AddSingleton<IMeetingMatcher, MeetingMatcher>();
        builder.Services.AddSingleton<ITeacherRankManager, TeacherRankManager>();
        builder.Services.AddSingleton<IUserStateChecker, UserStateChecker>();

        // builder.Services.AddSingleton<IMeetingHandler, ?>();
    }
}