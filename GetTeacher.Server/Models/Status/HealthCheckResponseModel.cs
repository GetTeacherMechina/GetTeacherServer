namespace GetTeacher.Server.Models.Status;

public class HealthCheckResponseModel
{
	public bool DatabaseConnectionStatus { get; set; } = false;
}