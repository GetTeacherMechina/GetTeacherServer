using GetTeacher.Server.DataStructures.Concurrent;
using GetTeacher.Server.Models.CsGoContract;
using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;

namespace GetTeacher.Server.Services.Managers.Implementations;

public record StudentEntry(DbStudent Student, DbSubject Subject, CancellationTokenSource StopMatchingCts);

public class MeetingMatcherBackgroundService(IServiceProvider serviceProvider, ILogger<MeetingMatcherBackgroundService> logger) : BackgroundService, IMeetingMatcherBackgroundService
{
	private const string csGoConfirm = "👍🏻";
	private const string csGoDeny = "👎🏿";

	private readonly IServiceProvider serviceProvider = serviceProvider;
	private readonly ILogger<MeetingMatcherBackgroundService> logger = logger;
	private readonly SemaphoreSlim trigger = new(0); // Semaphore for waking up the service

	private readonly ConcurrentList<StudentEntry> studentsProcessingQueue = [];
	private readonly ConcurrentList<StudentEntry> searchingStudents = [];

	public void StartMatchStudent(DbStudent student, DbSubject subject)
	{
		logger.LogInformation("Matching {studentName}, subject: {subjectName}.", student.DbUser.UserName, subject.Name);
		studentsProcessingQueue.Add(new StudentEntry(student, subject, new CancellationTokenSource()));

		// Release the semaphore to wake up the service
		trigger.Release();
	}

	public void StopMatchStudent(DbStudent student)
	{
		StudentEntry? studentEntry = searchingStudents.Where((s) => s.Student.Id == student.Id).FirstOrDefault();
		if (studentEntry is null)
			return;

		studentEntry.StopMatchingCts.Cancel();
		studentsProcessingQueue.Remove(studentEntry);
		searchingStudents.Remove(studentEntry);
		logger.LogInformation("Stopped matching {studentName}", student.DbUser.UserName);
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
				StartAsyncMatches();
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

	private async void StartAsyncMatches()
	{
		while (studentsProcessingQueue.Count != 0)
		{
			StudentEntry studentEntry = studentsProcessingQueue.First();
			studentsProcessingQueue.Remove(studentEntry);
			// TODO: !!!!!
			await ProcessStudentMatchAsync(studentEntry);
		}
	}

	private async Task ProcessStudentMatchAsync(StudentEntry studentEntry)
	{
		searchingStudents.Add(studentEntry);

		while (!studentEntry.StopMatchingCts.IsCancellationRequested)
		{
			ICollection<DbTeacher> teacherExclusions = [];

			IServiceScope serviceScope = serviceProvider.CreateScope();
			IMeetingMatcher meetingMatcher = serviceScope.ServiceProvider.GetRequiredService<IMeetingMatcher>();
			IWebSocketSystem webSocketSystem = serviceScope.ServiceProvider.GetRequiredService<IWebSocketSystem>();
			IUserStateTracker userStateTracker = serviceScope.ServiceProvider.GetRequiredService<IUserStateTracker>();
			ITeacherReadyManager teacherReadyManager = serviceScope.ServiceProvider.GetRequiredService<ITeacherReadyManager>();

			// TODO: Plop the GUID from the database-backed session entry we create beforehand
			string meetingGuid = Guid.NewGuid().ToString();

			bool studentOnline = true;
			DbTeacher? foundTeacher = null;
			while (foundTeacher is null)
			{
				if (!(studentOnline = userStateTracker.IsUserOnline(studentEntry.Student.DbUser)))
					break;

				foundTeacher = await meetingMatcher.MatchStudentTeacher(studentEntry.Student, studentEntry.Subject, teacherExclusions);

				// TODO: Change this shit
				if (foundTeacher is null)
				{
					try
					{
						await Task.Delay(500, studentEntry.StopMatchingCts.Token); // Nice
					}
					catch { }

					continue;
				}

				CsGoContractRequestModel csGoContractRequestModel = new CsGoContractRequestModel
				{
					TeacherBio = foundTeacher.Bio,
					TeacherRank = foundTeacher.Rank,
					MeetingResponseModel = new MeetingResponseModel
					{
						MeetingGuid = meetingGuid,
						CompanionName = foundTeacher.DbUser.UserName!,
					}
				};

				await webSocketSystem.SendAsync(studentEntry.Student.DbUserId, csGoContractRequestModel);
				ReceiveResult wsReadResult = await webSocketSystem.ReceiveAsync(studentEntry.Student.DbUserId);

				if (!wsReadResult.Success)
				{
					// TODO: Handle
					break;
				}

				string csgoConfirmOrDeny = wsReadResult.Message;
				if (csgoConfirmOrDeny != csGoConfirm && csgoConfirmOrDeny != csGoDeny)
				{
					// TODO: Handle
					break;
				}

				if (csgoConfirmOrDeny == csGoDeny)
				{
					teacherExclusions.Add(foundTeacher);
					foundTeacher = null;
				}
			}

			if (!studentOnline || foundTeacher is null)
				continue;

			teacherReadyManager.NotReadyToTeach(foundTeacher);
			// Notify student and teacher :)
			try
			{
				MeetingResponseModel meetingResponseModel = new MeetingResponseModel
				{
					MeetingGuid = meetingGuid,
					CompanionName = studentEntry.Student.DbUser.UserName!
				};

				await webSocketSystem.SendAsync(foundTeacher.DbUserId, meetingResponseModel);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unexpected expection happened when trying to match student and teacher.");
			}

			// TODO: Remove this shit with the while condition
			return;
		}

		searchingStudents.Remove(studentEntry);
	}
}