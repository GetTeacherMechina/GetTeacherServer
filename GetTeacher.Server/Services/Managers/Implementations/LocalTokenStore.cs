using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class LocalTokenStore(ICodeGenerator codeGenerator) : ITokenStore
{
	private static readonly Dictionary<string, string> codeToToken = [];
	private readonly ICodeGenerator codeGenerator = codeGenerator;

	public string CreateCodeForToken(string token)
	{
		string code = codeGenerator.GenerateCode();
		codeToToken.Add(code, token);
		return code;
	}

	public void RemoveCode(string token)
	{
		codeToToken.Remove(token);
	}

	public string? GetToken(string code)
	{
		if (codeToToken.TryGetValue(code, out string? token))
			return token;

		return null;
	}
}