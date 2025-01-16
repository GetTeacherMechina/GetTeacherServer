using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers;

public class CodeGenerator : ICodeGenerator
{
	public string GenerateCode()
	{
		return new Random().Next(10000, 100000).ToString();
	}
}