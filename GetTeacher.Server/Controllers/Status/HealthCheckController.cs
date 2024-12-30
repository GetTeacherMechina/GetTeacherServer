using GetTeacher.Server.Models.Status;
using GetTeacher.Server.Services.Database;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Status;

[ApiController]
[Route("api/v1/status/[controller]")]
public class HealthCheckController(ILogger<HealthCheckController> logger, DatabaseConnectionTester databaseConnectionTester) : ControllerBase
{
	private readonly ILogger<HealthCheckController> logger = logger;
	private readonly DatabaseConnectionTester databaseConnectionTester = databaseConnectionTester;

	[HttpGet]
	public async Task<IActionResult> GetHealthCheck()
	{
		logger.LogInformation("Starting health check...");
		bool databaseConnectionStatus = await databaseConnectionTester.TestConnectionAsync();
		logger.LogInformation("Health check completed");

		return Ok(new HealthCheckResponseModel { DatabaseConnectionStatus = databaseConnectionStatus });
	}
}