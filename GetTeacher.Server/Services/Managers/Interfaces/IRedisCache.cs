namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IRedisCache
{
	public Task AddToHashAsync(string hashKey, int id, string value);

	public Task<string?> GetFromHashAsync(string hashKey, int id);

	public Task RemoveFromHashAsync(string hashKey, int id);

	public Task<Dictionary<int, string>> GetAllEntriesFromHashAsync(string hashKey);

	public Task<ICollection<int>> GetAllKeysFromHashAsync(string hashKey);

	public Task<ICollection<string>> GetAllValuesFromHashAsync(string hashKey);
}