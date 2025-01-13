namespace GetTeacher.Server.Extensions;

public static class HostEnvironmentEnvExtensions
{
	public static bool IsRelease(this IHostEnvironment hostEnvironment)
	{
		return hostEnvironment.IsEnvironment("Release");
	}
}