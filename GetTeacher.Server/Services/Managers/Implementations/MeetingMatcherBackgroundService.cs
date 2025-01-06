using System.Collections.Concurrent;
using GetTeacher.Server.Models.CsGoContract;
using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;

namespace GetTeacher.Server.Services.Managers.Implementations;

public record StudentEntry(DbStudent Student, DbSubject Subject);

public class MeetingMatcherBackgroundService(IServiceProvider serviceProvider, ILogger<MeetingMatcherBackgroundService> logger) : BackgroundService, IMeetingMatcherBackgroundService
{
	private readonly IServiceProvider serviceProvider = serviceProvider;
	private readonly ILogger<MeetingMatcherBackgroundService> logger = logger;
	private readonly SemaphoreSlim trigger = new(0); // Semaphore for waking up the service

	private readonly ConcurrentQueue<StudentEntry> matchingStudents = new ConcurrentQueue<StudentEntry>();

	public void MatchStudent(DbStudent student, DbSubject subject)
	{
		logger.LogInformation("Matching {studentName}, subject: {subjectName}.", student.DbUser.UserName, subject.Name);
		matchingStudents.Enqueue(new StudentEntry(student, subject));

		// Release the semaphore to wake up the service
		trigger.Release();
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Student matcher background service has started running.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				// Wait for the trigger
				await trigger.WaitAsync(stoppingToken);

				logger.LogInformation("Student matcher background job is processing.");
				await ProcessMatchesAsync();
			}
			catch (OperationCanceledException)
			{
				logger.LogInformation("Exiting student matcher background service.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unexpected error occurred when matching students.");
			}
		}
	}

	private async Task ProcessMatchesAsync()
	{
		while (!matchingStudents.IsEmpty)
		{
			if (!matchingStudents.TryDequeue(out StudentEntry? studentEntry))
				return;

			IServiceScope serviceScope = serviceProvider.CreateScope();
			IMeetingMatcher meetingMatcher = serviceScope.ServiceProvider.GetRequiredService<IMeetingMatcher>();
			IWebSocketSystem webSocketSystem = serviceScope.ServiceProvider.GetRequiredService<IWebSocketSystem>();
			IUserStateTracker userStateTracker = serviceScope.ServiceProvider.GetRequiredService<IUserStateTracker>();

			bool studentOnline = true;
			DbTeacher? foundTeacher = null;
			while (foundTeacher is null)
			{
				if (!(studentOnline = userStateTracker.IsUserOnline(studentEntry.Student.DbUser)))
					break;

				foundTeacher = await meetingMatcher.MatchStudentTeacher(studentEntry.Student, studentEntry.Subject);
				await Task.Delay(500); // Nice
			}

			if (!studentOnline || foundTeacher is null)
				continue;

			// Notify student and teacher :)
			try
			{
				// TODO: Plop the GUID from the database-backed session entry we create beforehand
				string meetingGuid = Guid.NewGuid().ToString();
				CsGoContractRequestModel csGoContractRequestModel =	new CsGoContractRequestModel
				{
					TeacherBio = foundTeacher.Bio,
					TeacherRank = foundTeacher.Rank,
					MeetingResponseModel = new MeetingResponseModel
					{
						MeetingGuid = meetingGuid,
						CompanionName = studentEntry.Student.DbUser.UserName!,
					}
				};

				Task sendTeacher = webSocketSystem.SendAsync(foundTeacher.DbUserId, csGoContractRequestModel);

				csGoContractRequestModel.MeetingResponseModel.CompanionName = foundTeacher.DbUser.UserName!;
				Task sendStudent = webSocketSystem.SendAsync(studentEntry.Student.DbUserId, csGoContractRequestModel);
				await Task.WhenAll(sendTeacher, sendStudent);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unexpected expection happened when trying to match student and teacher.");
			}
		}
	}
}