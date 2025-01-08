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

	private readonly SemaphoreSlim matchSemaphore = new SemaphoreSlim(1, 1);
	private readonly ConcurrentList<StudentEntry> studentsProcessingQueue = [];
	private readonly ConcurrentList<StudentEntry> searchingStudents = [];
	private readonly ConcurrentList<DbTeacher> csGoPhaseTeachers = [];

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

	private void StartAsyncMatches()
	{
		while (studentsProcessingQueue.Count != 0)
		{
			StudentEntry studentEntry = studentsProcessingQueue.First();
			studentsProcessingQueue.Remove(studentEntry);
			// TODO: !!!!!
			_ = ProcessStudentMatchAsync(studentEntry);
		}
	}

	private async Task ProcessStudentMatchAsync(StudentEntry studentEntry)
	{
		searchingStudents.Add(studentEntry);

		IServiceScope serviceScope = serviceProvider.CreateScope();
		IWebSocketSystem webSocketSystem = serviceScope.ServiceProvider.GetRequiredService<IWebSocketSystem>();
		ITeacherReadyManager teacherReadyManager = serviceScope.ServiceProvider.GetRequiredService<ITeacherReadyManager>();

		// TODO: Plop the GUID from the database-backed session entry we create beforehand
		string meetingGuid = Guid.NewGuid().ToString();

		DbTeacher? teacher = await MatchTeacher(studentEntry, meetingGuid);
		if (teacher is null)
			return;

		teacherReadyManager.NotReadyToTeach(teacher);

		// Notify student and teacher :)
		try
		{
			MeetingResponseModel meetingResponseModel = new MeetingResponseModel
			{
				MeetingGuid = meetingGuid,
				CompanionName = studentEntry.Student.DbUser.UserName!
			};

			await webSocketSystem.SendAsync(teacher.DbUserId, meetingResponseModel);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An unexpected expection happened when trying to match student and teacher.");
		}

		searchingStudents.Remove(studentEntry);
	}

	private async Task<DbTeacher?> MatchTeacher(StudentEntry studentEntry, string meetingGuid)
	{
		IServiceScope serviceScope = serviceProvider.CreateScope();
		IMeetingMatcher meetingMatcher = serviceScope.ServiceProvider.GetRequiredService<IMeetingMatcher>();

		ICollection<DbTeacher> teacherExclusions = [];
		DbTeacher? foundTeacher = null;

		while (foundTeacher is null && !studentEntry.StopMatchingCts.IsCancellationRequested)
		{
			await matchSemaphore.WaitAsync();
			try
			{
				foundTeacher = await meetingMatcher.MatchStudentTeacher(studentEntry.Student, studentEntry.Subject, teacherExclusions.Concat(csGoPhaseTeachers).ToList());

				if (foundTeacher is null)
				{
					await Task.Delay(500);
					continue;
				}

				csGoPhaseTeachers.Add(foundTeacher);
			}
			finally
			{
				matchSemaphore.Release();
			}


			if (await CsGoContract(studentEntry, foundTeacher, meetingGuid))
				break;
			else
			{
				teacherExclusions.Add(foundTeacher);
				csGoPhaseTeachers.Remove(foundTeacher);
			}
		}

		if (studentEntry.StopMatchingCts.IsCancellationRequested)
			return null;

		csGoPhaseTeachers.Remove(foundTeacher!);
		return foundTeacher;
	}

	private async Task<bool> CsGoContract(StudentEntry studentEntry, DbTeacher teacher, string meetingGuid)
	{
		IServiceScope serviceScope = serviceProvider.CreateScope();
		IMeetingMatcher meetingMatcher = serviceScope.ServiceProvider.GetRequiredService<IMeetingMatcher>();
		IWebSocketSystem webSocketSystem = serviceScope.ServiceProvider.GetRequiredService<IWebSocketSystem>();

		CsGoContractRequestModel csGoContractRequestModel = new CsGoContractRequestModel
		{
			TeacherBio = teacher.Bio,
			TeacherRank = teacher.Rank,
			MeetingResponseModel = new MeetingResponseModel
			{
				MeetingGuid = meetingGuid,
				CompanionName = teacher.DbUser.UserName!,
			}
		};

		await webSocketSystem.SendAsync(studentEntry.Student.DbUserId, csGoContractRequestModel);
		ReceiveResult wsReadResult = await webSocketSystem.ReceiveAsync(studentEntry.Student.DbUserId);

		if (!wsReadResult.Success)
			return false;

		string csgoConfirmOrDeny = wsReadResult.Message;
		if (csgoConfirmOrDeny != csGoConfirm && csgoConfirmOrDeny != csGoDeny)
			return false;

		if (csgoConfirmOrDeny == csGoDeny)
			return false;

		if (csgoConfirmOrDeny == csGoConfirm)
			return true;

		return false;
	}
}