namespace GetTeacher.Server.Services.Database;

public class DatabaseConnectionTester(ILogger<DatabaseConnectionTester> logger, GetTeacherDbContext getTeacherDbContext)
{
	private readonly ILogger<DatabaseConnectionTester> logger = logger;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	public async Task<bool> TestConnectionAsync()
	{
		try
		{
			return await getTeacherDbContext.Database.CanConnectAsync();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Database connection test failed.");
			return false;
		}
	}
}