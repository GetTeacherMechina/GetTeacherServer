﻿using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Implementations;
using GetTeacher.Server.Services.Managers.Implementations.EmailSender;
using GetTeacher.Server.Services.Managers.Implementations.Networking;
using GetTeacher.Server.Services.Managers.Implementations.ReadyManager;
using GetTeacher.Server.Services.Managers.Implementations.UserManager;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GetTeacher.Server.Extensions.Builder;

public static class ServicesBuilderExtensions
{
	public static void AddGetTeacherServices(this WebApplicationBuilder builder)
	{
		builder.UseGetTeacherDb();

		builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
		builder.Services.AddScoped<ITeacherManager, TeacherManager>();
		builder.Services.AddScoped<IStudentManager, StudentManager>();
		builder.Services.AddScoped<ITeacherRankManager, TeacherRankManager>();
		builder.Services.AddScoped<IUserStateTracker, UserStateTracker>();
		builder.Services.AddScoped<ITeacherReadyManager, TeacherReadyManager>();
		builder.Services.AddScoped<IMeetingMatcher, MeetingMatcher>();
		builder.Services.AddScoped<IEmailSender, EmailSender>();
		builder.Services.AddScoped<IEmailSender<DbUser>, IdentityEmailSender>();

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