using GetTeacher.Server.Models;
using GetTeacher.Server.Services.Database;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthCheckController(ILogger<HealthCheckController> logger, DatabaseConnectionTester databaseConnectionTester) : ControllerBase
{
	private readonly ILogger<HealthCheckController> logger = logger;
	private readonly DatabaseConnectionTester databaseConnectionTester = databaseConnectionTester;

	[HttpGet]
    public async Task<IActionResult> Health()
    {
		logger.LogInformation("Starting health check...");
		bool databaseConnectionStatus = await databaseConnectionTester.TestConnectionAsync();
		logger.LogInformation("Ended health check!");

		return Ok(new HealthCheckResponseModel { DatabaseConnectionStatus = databaseConnectionStatus });
    }
}