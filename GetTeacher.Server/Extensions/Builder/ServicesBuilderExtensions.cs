using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers;
using GetTeacher.Server.Services.Managers.Implementations;
using GetTeacher.Server.Services.Managers.Implementations.Chats;
using GetTeacher.Server.Services.Managers.Implementations.EmailSender;
using GetTeacher.Server.Services.Managers.Implementations.Networking;
using GetTeacher.Server.Services.Managers.Implementations.Payment;
using GetTeacher.Server.Services.Managers.Implementations.ReadyManager;
using GetTeacher.Server.Services.Managers.Implementations.UserManager;
using GetTeacher.Server.Services.Managers.Implementations.UserStateTracker;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Chats;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GetTeacher.Server.Extensions.Builder;

public static class ServicesBuilderExtensions
{
	public static void AddGetTeacherServices(this WebApplicationBuilder builder)
	{
		builder.UseGetTeacherDb();
		builder.UseGetTeacherUserStateTracker();

		builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
		builder.Services.AddScoped<ITeacherManager, TeacherManager>();
		builder.Services.AddScoped<IStudentManager, StudentManager>();
		builder.Services.AddScoped<IUserManager, UserManager>();
		builder.Services.AddScoped<ITeacherRankManager, TeacherRankManager>();
		builder.Services.AddScoped<IUserStateTracker, UserStateTracker>();
		builder.Services.AddScoped<ITeacherReadyManager, TeacherReadyManager>();
		builder.Services.AddScoped<IStudentReadyManager, StudentReadyManager>();
		builder.Services.AddScoped<ITokenStore, LocalTokenStore>();
		builder.Services.AddScoped<IMeetingMatcherAlgorithm, MeetingMatcherAlgorithm>();
		builder.Services.AddScoped<IEmailSender, EmailSender>();
		builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();
		builder.Services.AddScoped<IEmailSender<DbUser>, IdentityEmailSender>();
		builder.Services.AddScoped<ISubjectManager, SubjectManager>();
		builder.Services.AddScoped<IGradeManager, GradeManager>();
		builder.Services.AddScoped<IMeetingManager, MeetingManager>();
		builder.Services.AddScoped<IUserCreditManager, UserCreditManager>();
		builder.Services.AddScoped<ICreditCharger, CreditCharger>();
		builder.Services.AddScoped<IPaymentIntentToCredits, LocalPaymentIntentToCredits>();
		builder.Services.AddScoped<IChatManager, ChatManager>();
		builder.Services.AddScoped<IStudentReadyTeacherCountNotifier, StudentReadyTeacherCountNotifier>();
		builder.Services.AddScoped<ITwoFactorAuthenticationManager, TwoFactorAuthenticationManager>();

		// Payment
		builder.Services.AddScoped<IItemPriceQuerier, LocalItemPriceQuerier>();
		builder.Services.AddScoped<IPaymentManager, StripePaymentManager>();

		// Testers
		builder.Services.AddScoped<DatabaseConnectionTester>();

		// Probably will only be used for manual JWT authentication in the context of WebSockets
		builder.Services.AddScoped<IJwtAuthenticator, JwtAuthenticator>();
		builder.Services.AddScoped<IWebSocketSystem, WebSocketSystem>();
		builder.Services.AddSingleton<IPrincipalClaimsQuerier, PrincipalClaimsQuerier>();

		// Add the meeting matcher as a background service
		builder.Services.AddSingleton<MeetingMatcherBackgroundService>();
		builder.Services.AddSingleton<IMeetingMatcherBackgroundService>(provider => provider.GetRequiredService<MeetingMatcherBackgroundService>());
		builder.Services.AddHostedService(provider => provider.GetRequiredService<MeetingMatcherBackgroundService>());
	}
}