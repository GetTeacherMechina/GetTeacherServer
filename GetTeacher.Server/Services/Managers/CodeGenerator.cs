using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers;

public class CodeGenerator : ICodeGenerator
{
	private readonly ICollection<int> generatedCodes = [];

	public string GenerateCode()
	{
		int code = new Random().Next(10000, 100000);
		if (generatedCodes.Contains(code))
			return GenerateCode();

		generatedCodes.Add(code);
		return code.ToString();
	}
}