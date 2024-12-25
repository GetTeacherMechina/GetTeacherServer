using GetTeacher.Server.Services.Generators;
using GetTeacher.Server.Services.Managers.Implementations;
using GetTeacher.Server.Services.Managers.Implementations.Networking;
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

		builder.Services.AddScoped<ITeacherManager, TeacherManager>();
		builder.Services.AddScoped<IStudentManager, StudentManager>();
		builder.Services.AddScoped<IMeetingMatcher, MeetingMatcher>();
		builder.Services.AddScoped<ITeacherRankManager, TeacherRankManager>();
		builder.Services.AddScoped<IUserStateChecker, UserStateChecker>();
		builder.Services.AddScoped<ITeacherReadyManager, TeacherReadyManager>();
		builder.Services.AddScoped<IStudentMatcher, StudentMatcher>();

		// Probably will only be used for manual JWT authentication in the context of WebSockets
		builder.Services.AddScoped<IWebSocketHandler, WebSocketHandler>();
		builder.Services.AddScoped<IJwtAuthenticator, JwtAuthenticator>();
		builder.Services.AddSingleton<IPrincipalClaimsQuerier, PrincipalClaimsQuerier>();

		// builder.Services.AddSingleton<IMeetingHandler, ?>();
	}
}